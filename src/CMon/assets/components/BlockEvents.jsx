import React from "react";

import * as fetcher from "../fetcher";

import Panel from "./Panel"

class BlockEvents extends React.Component {
	constructor(props) {
		super(props);
		this.state = { items: [] };
		this.onApplyFilter = this.onApplyFilter.bind(this);
	}
	componentDidMount() {
		window.addEventListener("apply-filter", this.onApplyFilter);
	}
	componentWillUnmount() {
		window.removeEventListener("apply-filter", this.onApplyFilter);
	}
	onApplyFilter(e) {
		this.requestEvents(e.detail);
	}
	requestEvents(range) {
		fetcher.post("/api/device/events",
			{
				deviceId: this.props.deviceId,
				from: range.from,
				to: range.to
			},
			(err, res) => {
				this.setState(res.body);
			}
		);
	}
	render() {
		return (
			<Panel span="6" title={this.props.name}>
				{this.state.items.map(item => {
					return (
						<div key={item.id} className="row">
							<div className="col-md-1">
								<span className="badge">{item.externalId}</span>
							</div>							
							<div className="col-md-4">
								{item.createdAt}
							</div>
							<div className="col-md-2">
								<div className="media-left media-middle">
									{item.eventType}
								</div>
							</div>
							<div className="col-md-5">
								<code style={{wordBreak: 'break-all'}}>{JSON.stringify(item.info)}</code>
							</div>
						</div>
					);
				})}
			</Panel>
		);
	}
}

module.exports = BlockEvents;