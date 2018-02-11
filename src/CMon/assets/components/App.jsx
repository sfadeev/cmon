import React from "react";
import ReactDOM from "react-dom";

import BlockEvents from "./BlockEvents"

const App = (props) => (
	<div className="col-md-6">
		<BlockEvents deviceId={props.deviceId} />
	</div>
);

var container = document.getElementById("app"),
	deviceId = container.getAttribute("data-device-id");

ReactDOM.render(<App deviceId={deviceId} />, container);