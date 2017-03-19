require('bootstrap-webpack!./bootstrap.config.js');

require('./common');
require('./dashboard');

// require('expose?$!expose?jQuery!jquery');
window.$ = require('jquery'); // todo: remove, required for chart
