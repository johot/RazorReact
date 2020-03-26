using JavaScriptEngineSwitcher.Core;
using System;
using System.Runtime.Caching;
using JavaScriptEngineSwitcher.ChakraCore;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace RazorReact.Core
{
    public abstract class RazorReactManagerBase : IRazorReactManager
    {
        private ObjectCache cache = MemoryCache.Default;

        public ReactBundle ReactBundle { get; }
        public RazorReactOptions Options { get; } = new RazorReactOptions();

        private IJsEngine _jsEngine;
        private IJsEngineFactory _jsEngineFactory;

        private readonly IServerPathMapper _mapServerPath;

        protected RazorReactManagerBase(ReactBundle reactBundle, IServerPathMapper mapServerPath, IJsEngineFactory jsEngineFactory, RazorReactOptions options = null)
        {
            if (options != null)
            {
                Options = options;
            }

            _jsEngineFactory = jsEngineFactory;

            _mapServerPath = mapServerPath;

            ReactBundle = reactBundle;

            // TODO: JsPool
            //JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName; // V8JsEngine.EngineName;
            //JsEngineSwitcher.Current.EngineFactories.AddChakraCore(); //.AddV8();
            //JsEngineSwitcher.Current.CreateDefaultEngine();
        }

        public void Initialize()
        {
            _jsEngine = _jsEngineFactory.CreateEngine();

            // Execute each bundle
            foreach (var reactBundleFile in ReactBundle.BundleFiles)
            {
                // Fix console is undefined errors
                _jsEngine.Evaluate("if (typeof console === 'undefined') console = { log: function() {}, error: function() {} };");

                if (reactBundleFile.ToLowerInvariant().StartsWith("http"))
                {
                    var contents = new HttpClient().GetStringAsync(reactBundleFile).Result;

                    _jsEngine.Evaluate(contents);

                    //var newCode = _jsEngine.Precompile(contents);
                    //_jsEngine.Execute(newCode);
                }
                else
                {
                    _jsEngine.ExecuteFile(_mapServerPath.MapServerPath(reactBundleFile), null);
                }
            }
        }

        public string GetClientSideRenderScripts(string componentName, object props, string bundleId = null, string containerId = null)
        {
            if (!Options.ClientSide)
                return $"<!-- Client side rendering disabled for: {componentName} -->";

            var propsAsString = GetPropsAsStringOrNull(props);

            var id = GetContainerId(containerId, componentName);

            var scriptTag = new StringBuilder();

            foreach (var bundleFile in ReactBundle.BundleFiles)
            {
                // Remove start ~ character (used in ASP.NET)
                var scriptSrc = Regex.Replace(bundleFile, "^~", "");
                scriptTag.AppendLine($"<script src=\"{scriptSrc}\"></script>");
            }

            scriptTag.AppendLine($"<script>ReactDOM.hydrate(React.createElement({componentName}, {propsAsString}), document.getElementById(\"{id}\"))</script>");

            return scriptTag.ToString();
        }

        private string GetContainerId(string userSupplidId, string componentName)
        {
            if (userSupplidId != null)
            {
                return userSupplidId;
            }

            return componentName + "-root";
        }

        private string GetPropsAsStringOrNull(object props)
        {
            if (props == null)
                return null;

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(props, serializerSettings);
        }

        public string GetServerSideRenderedHtml(string componentName, object props, string bundleId = null, string containerId = null)
        {
            var comments = new StringBuilder();

            // For live reload dev mode reset everything each render
            if (Options.LiveReloadDevMode)
            {
                Initialize();
                Options.CacheRendering = false;

                comments.AppendLine("<!-- Live reload dev mode enabled, performance will be lower -->");
            }

            var id = GetContainerId(containerId, componentName);
            var propsAsString = GetPropsAsStringOrNull(props);

            var cacheKey = $"{id}-{propsAsString}";

            object html = null;

            if (Options.CacheRendering)
            {
                html = cache[cacheKey];
            }

            if (html == null)
            {
                var outputHtml = new StringBuilder();
                outputHtml.Append($"<div id=\"{id}\">");

                if (Options.ServerSide)
                {
                    var ssrHtml = _jsEngine.Evaluate($"ReactDOMServer.renderToString(React.createElement({componentName}, {propsAsString}))");
                    outputHtml.Append(ssrHtml.ToString());
                }

                outputHtml.Append("</div>");

                var finalHtml = outputHtml.ToString();

                // Set cached rendering if enabled (default enabled)
                if (Options.CacheRendering)
                {
                    cache.Set(cacheKey, finalHtml, new CacheItemPolicy());
                }

                return comments.ToString() + finalHtml;
            }
            else
            {
                comments.AppendLine($"<!-- Rendering cached: {componentName} -->");

                // Cached version
                return comments.ToString() + html.ToString();
            }
        }

    }
}
