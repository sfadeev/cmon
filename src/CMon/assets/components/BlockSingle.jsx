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
		if ((`${e.detail.deviceId}`) === this.props.deviceId && e.detail.inputNo === this.props.inputNo) {
			this.setState({ value: e.detail.temp.toFixed(2) });
		}
	}
	render() {
		return (
			<Panel span="2" title={this.props.name}>
				<div className="single">
					<span className="value">{this.state.value || "--"}</span>
				</div>
			</Panel>
		);
	}
}