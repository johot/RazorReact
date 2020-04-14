const path = require("path");
const ManifestPlugin = require("webpack-manifest-plugin");
const BundleAnalyzerPlugin = require("webpack-bundle-analyzer").BundleAnalyzerPlugin;

// *** I had a lot of issues getting Emotion 10 to work since it would assume this code will only run in the browser, but with some tricks I got it working anyway
// by setting the aliasFields so that Emotion 10 won't use the browser built version of it's package /Johan

// https://github.com/emotion-js/emotion/issues/1246
// https://github.com/emotion-js/emotion/issues/1728
// https://github.com/reactjs/React.NET/issues/970
// https://reactjs.net/bundling/webpack.html?fb_comment_id=1818032604981502_1955772601207501

export function generateWebpackConfig(
  serverBundle: boolean,
  mode: "production" | "development",
  analyze: boolean,
  assetPath: string,
  debugErrors: boolean
): any {
  //console.log("Running webpack with arguments", "env:", env, "argv:", argv);

  //const serverBundle = !!argv.serverBundle;
  //const debug = !!argv.debug;

  //const mode = argv.production ? "production" : "development";

  if (debugErrors && mode !== "production")
    throw new Error("The debug errors option can only be used in production builds currently");

  const presetEnvOptions = serverBundle
    ? {
        targets: {
          //chrome: "80",
          edge: 16, // When using ChakraCore as the JS Engine
        },
      }
    : {};

  // I can't get this working in our web environment, maybe it will mostly work for a node target, but we can get it working by just using regenerator-runtime instead
  const clientBundlePlugins: any[] = [];
  //  !serverBundle
  //   ? [
  //       [
  //         "@babel/plugin-transform-runtime",
  //         {
  //           regenerator: true,
  //         },
  //       ],
  //     ]
  //   : [];

  return {
    // Change to your "entry-point".
    entry: serverBundle
      ? [path.resolve(__dirname, "initServer.js"), path.resolve("./src/razorReact.ts")]
      : ["regenerator-runtime/runtime", path.resolve(__dirname, "initClient.js"), path.resolve("./src/razorReact.ts")], // Regenerator runtime only needed for client since we build for a very modern JS engine on server
    mode: mode,
    target: "web",
    output: {
      publicPath: assetPath, // For assets
      path: path.resolve("./dist", serverBundle ? "server" : "client"),
      filename: "[name].bundle.js",
      globalObject: "this",
    },
    //devtool: false,

    resolve: {
      extensions: [".ts", ".tsx", ".js", ".json"],

      // When creating this bundle we will use the module fields of our packages instead of the browser or main fields
      // this is because there is an issue with Emotion 10 where SSR will not work if we use target: web because it automatically assumes
      // we are a browser, I have commented here: https://github.com/emotion-js/emotion/issues/1246
      aliasFields: ["module"],
      //mainFields: ["module", "main"] ... this didn't work but aliasFields did, not sure about the difference /Johan
    },
    optimization: {
      minimize: debugErrors ? false : undefined,
      runtimeChunk: {
        name: "runtime", // necessary when using multiple entrypoints on the same page
      },
      // Read more here: https://survivejs.com/webpack/building/bundle-splitting/
      splitChunks: {
        // cacheGroups: {
        //   commons: {
        //     test: /[\\/]node_modules[\\/](react|react-dom)[\\/]/,
        //     name: "vendor",
        //     chunks: "all"
        //   }
        // }
        chunks: "all",
      },
    },

    module: {
      rules: [
        {
          // Include ts, tsx, js, and jsx files.
          test: /\.(ts|js)x?$/,
          //exclude: /node_modules/,
          use: {
            loader: "babel-loader",
            options: {
              presets: [["@babel/preset-env", presetEnvOptions], "@babel/react", "@babel/typescript"],
              //plugins: ["@babel/plugin-transform-runtime"],
              //These are only needed if using @babel/preset-env < 7.8
              //sourceType: "unambiguous",
              plugins: [
                "@babel/plugin-proposal-nullish-coalescing-operator",
                "@babel/plugin-proposal-optional-chaining",
                ...clientBundlePlugins,
              ],
            },
          },
        },
        // *** Needs more work to work with SSR so disabling css import support now ***
        // {
        //   test: /\.css$/,

        //   use: [
        //     "isomorphic-style-loader",
        //     {
        //       loader: "css-loader",
        //       options: {
        //         importLoaders: 1,
        //       },
        //     },
        //   ],
        // },
        {
          test: /\.(png|svg|jpg|gif)$/,
          use: {
            loader: "file-loader",
          },
        },
      ],
    },
    // externals: {
    //   "react-dom/server": "ReactDOMServer"
    // },
    plugins: [
      new ManifestPlugin({
        fileName: "asset-manifest.json",
        generate: (seed: any, files: any) => {
          const manifestFiles = files.reduce((manifest: any, file: any) => {
            manifest[file.name] = file.path;
            return manifest;
          }, seed);

          const entrypointFiles = files
            .filter((x: any) => x.isInitial && !x.name.endsWith(".map"))
            .map((x: any) => x.path);

          return {
            files: manifestFiles,
            entrypoints: entrypointFiles,
          };
        },
      }),
      ...(analyze === true ? [new BundleAnalyzerPlugin()] : []),
    ],

    // devServer: {
    // 	contentBase: path.join(__dirname, "dist"),
    // 	clientLogLevel: "silent",
    // 	port: 5000,
    // },
  };
}
