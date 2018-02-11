import React from "react";
import request from "superagent";
import Panel from "./Panel"

class BlockEvents extends React.Component {
	constructor(props) {
		super(props);
		this.state = {
			items: []
		};
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
		console.log("Requesting events", range);

		var deviceId = this.props.deviceId,
			xsrfToken = document.querySelector("input[name=__RequestVerificationToken]").value;

		request
			.post("/api/device/events")
			.set("X-XSRF-TOKEN", xsrfToken)
			.query({
				deviceId: deviceId,
				from: range.from,
				to: range.to
			})
			.end((err, res) => {
				this.setState(res.body);

				/*if (res.ok) {
					var types = res.body || [];
					this.setState({ classifierTypes: types });
					if (types.length > 0) this.setFilterField("typeId", types[0].Id);
				}
				else if (res.body) {
					this.setState({ error: res.body });
				}
				else if (err) {
					this.setState({ errors: [err.message] });
				}*/
			});
	}
	render() {
		return (
			<Panel span="6">
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