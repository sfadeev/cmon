import React from "react";
import ReactDOM from "react-dom";

import * as fetcher from "../fetcher";

const BlockTypes = {
	events: require("./BlockEvents"),
	single: require("./BlockSingle")
};

class App extends React.Component {
	constructor(props) {
		super(props);
		this.state = { blocks: [] };
	}
	componentDidMount() {
		fetcher.post("/api/device/blocks",
			{
				deviceId: this.props.deviceId
			},
			(err, res) => {
				this.setState({
					blocks: res.body.blocks
				});
			});
	}
	render() {
		return this.state.blocks.map(item => {
			return React.createElement(BlockTypes[item.type],
				Object.assign({}, item, { deviceId: this.props.deviceId }));
		});
	}
}

var container = document.getElementById("app"),
	deviceId = container.getAttribute("data-device-id");

ReactDOM.render(<App deviceId={deviceId} />, container);