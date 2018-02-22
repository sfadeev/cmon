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
				<div className="events">
					{this.state.items.map(item => {
						return (
							<div key={item.id} className="media">
								<div className="media-left icon">
									<i className={"fa " + item.displayIcon} />
								</div>
								<div className="media-body">
									<div className="date">
										<DateTime date={item.createdAt} />
									</div>
									<p title={item.eventType}><strong>{item.displayTitle}</strong> <span className="badge">{item.externalId}</span></p>
									<code style={{wordBreak: "break-all"}}>{JSON.stringify(item.info)}</code>
								</div>
							</div>
						);
					})}
				</div>
			</Panel>
		);
	}
}

class DateTime extends React.Component {
	render() {
		return (
			<time data-date={this.props.date}>
				{new Date(this.props.date + "Z").toLocaleString("ru")}
			</time>
		);
	}
}

module.exports = BlockEvents;