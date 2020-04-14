const path = require("path");
const ManifestPlugin = require("webpack-manifest-plugin");
const BundleAnalyzerPlugin = require("webpack-bundle-analyzer").BundleAnalyzerPlugin;

// *** I had a lot of issues getting Emotion 10 to work since it would assume this code will only run in the browser, but with some tricks I got it working anyway
// by setting the aliasFields so that Emotion 10 won't use the browser built version of it's package /Johan

// https://github.com/emotion-js/emotion/issues/1246
// https://github.com/emotion-js/emotion/issues/1728
// https://github.com/reactjs/React.NET/issues/970
// https://reactjs.net/bundling/webpack.html?fb_comment_id=1818032604981502_1955772601207501

module.exports = (env, argv) => {
  console.log("Running webpack with arguments", "env:", env, "argv:", argv);

  const analyze = argv.analyze ? true : false;
  const clientBuild = false;

  const mode = argv.production ? "production" : "development";

  return {
    // Change to your "entry-point".
    entry: clientBuild ? path.resolve("./src/epiIntegration.ts") : path.resolve("./src/epiIntegrationServer.ts"),
    mode: mode,
    target: "web",
    output: {
      //publicPath: "/ClientUI/js/react/", // For assets
      path: path.resolve("./dist"),
      filename: "[name].bundle.js",
      globalObject: "this",
    },
    resolve: {
      extensions: [".ts", ".tsx", ".js", ".json"],

      // When creating this bundle we will use the module fields of our packages instead of the browser or main fields
      // this is because there is an issue with Emotion 10 where SSR will not work if we use target: web because it automatically assumes
      // we are a browser, I have commented here: https://github.com/emotion-js/emotion/issues/1246
      aliasFields: ["module"],
      //mainFields: ["module", "main"] ... this didn't work but aliasFields did, not sure about the difference /Johan
    },
    optimization: {
      // minimize: minimize,
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
              presets: ["@babel/preset-env", "@babel/react", "@babel/typescript"],
            },
          },
        },
        {
          test: /\.css$/,
          use: ["style-loader", "css-loader"],
        },
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
        generate: (seed, files) => {
          const manifestFiles = files.reduce((manifest, file) => {
            manifest[file.name] = file.path;
            return manifest;
          }, seed);

          const entrypointFiles = files.filter((x) => x.isInitial && !x.name.endsWith(".map")).map((x) => x.path);

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
};
