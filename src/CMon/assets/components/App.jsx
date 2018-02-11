import React from "react";
import ReactDOM from "react-dom";

import BlockEvents from "./BlockEvents"
import BlockSingle from "./BlockSingle"

class App extends React.Component {
	render() {
		return ([
			<BlockEvents deviceId={this.props.deviceId } />,
			<BlockSingle deviceId={this.props.deviceId } />,
			<BlockSingle deviceId={this.props.deviceId } />,
			<BlockSingle deviceId={this.props.deviceId } />,
			<BlockSingle deviceId={this.props.deviceId } />,
			<BlockSingle deviceId={this.props.deviceId } />,
			<BlockSingle deviceId={this.props.deviceId } />,
			<BlockSingle deviceId={this.props.deviceId } />
		]);
	}
}

var container = document.getElementById("app"),
	deviceId = container.getAttribute("data-device-id");

ReactDOM.render(<App deviceId={deviceId} />, container);