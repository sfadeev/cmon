import React from "react";
import Panel from "./Panel"

module.exports = class BlockEvents extends React.Component {
	constructor(props) {
		super(props);
		this.state = { value: null };

		this.onInputTemperature = this.onInputTemperature.bind(this);
	}
	componentDidMount() {
		window.addEventListener("input-temperature", this.onInputTemperature);
	}
	componentWillUnmount() {
		window.removeEventListener("input-temperature", this.onInputTemperature);
	}
	onInputTemperature(e) {
		if ((`${e.detail.deviceId}`) === this.props.deviceId && (`${e.detail.inputNo}`) === this.props.name) {
			this.setState({ value: Math.round(e.detail.temp * 100) / 100 });
		}
	}
	render() {
		return (
			<Panel span="1" title={this.props.name}>
				<h3>{this.state.value || "--"}</h3>
			</Panel>
		);
	}
}