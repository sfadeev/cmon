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
		console.log("BlockEvents.componentDidMount");
		window.addEventListener("apply-filter", this.onApplyFilter);
	}
	componentWillUnmount() {
		console.log("BlockEvents.componentWillUnmount");
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
				<div className="m-list">
					{this.state.items.map(item => {
						return (
							<div key={item.id} className="row m-list-item">
								<div className="col-sm-12">
									<div className="m-list-item-icon">
										<i className={`fa fa-fw fa-lg fa-${item.displayIcon}`}/>
									</div>
									<div className="m-list-item-content">
										<div><strong>{item.displayTitle}</strong></div>
										<DateTime date={item.createdAt}/>
										{ item.displayParams && (
											<div>
												{ item.displayParams.map(x => `${x.name}: ${x.value}`).join(", ") }
											</div>
										)}
									</div>

									<p>
										<code style={{wordBreak: "break-all"}}>
											{item.eventType}#{item.externalId} - {JSON.stringify(item.info)}
										</code>
									</p>

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

		const locale = "ru";

		return (
			<time data-date={this.props.date}>
				{new Date(this.props.date + "").toLocaleDateString(locale)}
				<span> · </span>
				{new Date(this.props.date + "").toLocaleTimeString(locale)}
			</time>
	
		);
	}
}

module.exports = BlockEvents;