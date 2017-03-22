var path = require('path'),
    webpack = require("webpack"),
    extractText = require("extract-text-webpack-plugin"),
    uglifyjs = require('uglifyjs-webpack-plugin');

const extractLess = new extractText({
    filename: "[name].css",
    disable: process.env.NODE_ENV === "development"
});

module.exports = {
    entry: { 
        scripts: "./assets/index.js",
        styles: "./assets/index.less"
    },
    output: {
        path: path.join(__dirname, "wwwroot/build"),
        filename: "[name].js"
    },
    module: {
        rules: [{
                test: /\.less$/,
                use: extractLess.extract({
                    use: [{
                        loader: "css-loader"
                    }, {
                        loader: "less-loader"
                    }],
                    // use style-loader in development
                    fallback: "style-loader"
                })
            },
            { test: /bootstrap\/js\//, loader: 'imports?jQuery=jquery' },
			// Needed for the css-loader when [bootstrap-webpack](https://github.com/bline/bootstrap-webpack)
			// loads bootstrap's css.
			{ test: /\.(woff|woff2)(\?.*$|$)/, loader: "url-loader?limit=10000&mimetype=application/font-woff" },
			{ test: /\.ttf(\?.*$|$)/, loader: "file-loader" },
			{ test: /\.eot(\?.*$|$)/, loader: "file-loader" },
			{ test: /\.svg(\?.*$|$)/, loader: "file-loader" },
            // { test: /\.(png|woff|woff2|eot|ttf|svg)$/, loader: 'url-loader?limit=100000' }
        ]
    },
    plugins: [
        // new uglifyjs(),
        new webpack.ProvidePlugin({
          $: "jquery",
          jQuery: "jquery",
          "window.jQuery": "jquery"
        }),
        extractLess
    ],
    resolve: {
        extensions: [ '.js', '.jsx' ]
    }
};