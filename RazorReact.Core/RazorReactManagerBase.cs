﻿using JavaScriptEngineSwitcher.Core;
using System;
using System.Runtime.Caching;
using JavaScriptEngineSwitcher.ChakraCore;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RazorReact.Core
{
    public abstract class RazorReactManagerBase : IRazorReactManager
    {
        private ObjectCache cache = MemoryCache.Default;

        public IEnumerable<ReactBundle> ReactBundles { get; }

        private IJsEngine _jsEngine;
        private readonly IServerPathMapper _mapServerPath;

        protected RazorReactManagerBase(IEnumerable<ReactBundle> reactBundles, IServerPathMapper mapServerPath, IJsEngine jsEngine)
        {
            _mapServerPath = mapServerPath;

            ReactBundles = reactBundles;

            // TODO: JsPool
            //JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName; // V8JsEngine.EngineName;
            //JsEngineSwitcher.Current.EngineFactories.AddChakraCore(); //.AddV8();
            //JsEngineSwitcher.Current.CreateDefaultEngine();

            _jsEngine = jsEngine;

            // Execute each bundle
            foreach (var reactBundle in ReactBundles)
            {
                foreach (var reactBundleFile in reactBundle.BundleFiles)
                {
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
        }

        public string GetClientSideRenderScripts(string componentName, object props = null, RazorReactOptions razorReactOptions = null)
        {
            var options = razorReactOptions == null ? new RazorReactOptions() : razorReactOptions;

            if (!options.ClientSide)
                return $"<!-- Client side rendering disabled for: {componentName} -->";

            var propsAsString = GetPropsAsStringOrNull(props);

            var id = GetContainerId(options?.ContainerId, componentName);

            var reactBundle = ReactBundles.First();

            var scriptTag = new StringBuilder();

            foreach (var bundleFile in reactBundle.BundleFiles)
            {
                scriptTag.AppendLine($"<script src=\"{bundleFile.Replace("~", "")}\"></script>");
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

            return JsonConvert.SerializeObject(props);
        }

        public string GetServerSideRenderedHtml(string componentName, object props = null, RazorReactOptions razorReactOptions = null)
        {
            var options = razorReactOptions == null ? new RazorReactOptions() : razorReactOptions;

            var id = GetContainerId(options.ContainerId, componentName);
            var propsAsString = GetPropsAsStringOrNull(props);

            var cacheKey = $"{id}-{propsAsString}";

            try
            {
                object html = null;

                if (options.CacheRendering)
                {
                    html = cache[cacheKey];
                }

                if (html == null)
                {
                    var outputHtml = new StringBuilder();
                    outputHtml.Append($"<div id=\"{id}\">");

                    if (options.ServerSide)
                    {
                        var ssrHtml = _jsEngine.Evaluate($"ReactDOMServer.renderToString(React.createElement({componentName}, {propsAsString}))");
                        outputHtml.Append(ssrHtml.ToString());
                    }

                    outputHtml.Append("</div>");

                    var finalHtml = outputHtml.ToString();

                    // Set cached rendering if enabled (default enabled)
                    if (options.CacheRendering)
                    {
                        cache.Set(cacheKey, finalHtml, new CacheItemPolicy());
                    }

                    return finalHtml;
                }
                else
                {
                    // Cached version
                    return $"<!-- Rendering cached: {componentName} -->" + html.ToString();
                }
            }
            catch (Exception ex)
            {

                return ex.ToString();
            }
        }
    }
}
