# Razor React Bundler

Transpiles source code to be used with Razor React in a ASP.NET web page server + client side!

## Installation

In your React project (you can use something like `create-react-app` for testing locally):

```
npm i style-loader css-loader file-loader babel-loader @babel/core @babel/preset-env razor-react-bundler --save-dev
```

> Note: The reason we manually install a bunch of loaders is that in some projects like create-react-app they already include their own loaders which can conflict with ours. Maybe this can be fixed in a better way in the future

## Usage

Build a client + server side bundle:

```
npx rrb pack --production
```

## Notes and links

### Getting async working

TLDR: Install and use `regenerator-runtime/runtime` either in the webpack entry (which we do here) or by manually installing it

https://stackoverflow.com/questions/33527653/babel-6-regeneratorruntime-is-not-defined
