﻿import React from "react";

import * as fetcher from "../fetcher";

import Panel from "./Panel"
import Pagination from "./Pagination"

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
			<Panel span="5" title={this.props.name} className="panel-events">
				<div className="list-group">
					{this.state.items.map(item => {
						return (
							<a key={item.id} className="list-group-item">
								<div className="list-group-item-icon">
									<i className={`fa fa-fw fa-lg fa-${item.displayIcon}`} title={`${item.eventType} #${item.externalId}`} />
								</div>
								<div className="list-group-item-content">

									<div className="date">
										<DateTime date={item.createdAt}/>
									</div>

									<div className="title">
										{item.displayTitle}
									</div>

									{ item.displayParams && (
										<div>
											{ item.displayParams.map(x => `${x.name}: ${x.value}`).join(", ") }
										</div>
									)}

								</div>
							</a>
						);
					})}
				</div>
				<Pagination totalCount={ this.state.items.length } />
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
				<span> </span>
				{new Date(this.props.date + "").toLocaleTimeString(locale)}
			</time>
	
		);
	}
}

module.exports = BlockEvents;