#!/usr/bin/env node
import program from "commander";
import webpack from "webpack";
import chalk from "chalk";

import { generateWebpackConfig } from "./webpack.config";

const packageJson = require("../package.json");

program.version(packageJson.version);

program
  .command("pack")
  .description("Build bundles for use with RazorReact in ASP.NET")
  .option("-P, --production", "Create a production build")
  .option("-A, --analyze", "Analyze bundles after build")
  .option("-D, --debugErrors", "Create a build that can be more easily debugged for errors")
  .option("--assetPath <path>", "Set the asset path to use, defaults to /")
  .action((options: any) => {
    packProject(
      options.production ? "production" : "development",
      options.analyze,
      options.assetPath,
      options.debugErrors
    );
  });

function packProject(
  mode: "production" | "development",
  analyzeBundles: boolean,
  assetPath: string,
  debugErrors: boolean
) {
  console.log(chalk.magentaBright("------------------ Building Razor React server bundle! ------------------ "));

  // Server build
  webpack(
    generateWebpackConfig(true, mode, analyzeBundles, assetPath ? assetPath : "/", debugErrors),
    (err: any, stats: any) => {
      if (err) {
        console.error(err);
        return;
      }

      console.log(
        stats.toString({
          chunks: false, // Makes the build much quieter
          colors: true, // Shows colors in the console
        })
      );

      // Done! Start next
      console.log(chalk.magentaBright("------------------ Building Razor React client bundle! ------------------ "));

      // Client build
      webpack(
        generateWebpackConfig(false, mode, analyzeBundles, assetPath ? assetPath : "/", debugErrors),
        (err: any, stats: any) => {
          if (err) {
            console.error(err);
            return;
          }

          console.log(
            stats.toString({
              chunks: false, // Makes the build much quieter
              colors: true, // Shows colors in the console
            })
          );

          // All done!
          console.log(chalk.greenBright("All done!"));
        }
      );
    }
  );
}

program.parse(process.argv);

// Check the program.args obj
var NO_COMMAND_SPECIFIED = program.args.length === 0;

// Handle it however you like
if (NO_COMMAND_SPECIFIED) {
  // e.g. display usage
  program.help();
}
