import React from "react";
import request from "superagent";

class Events extends React.Component {
	constructor(props) {
		super(props);
		this.state = {
			items: []
		};
	}
	componentDidMount() {
		window.addEventListener("apply-filter", (e) => {
			this.requestEvents(e.detail);
		});
	}
	componentWillUnmount() {
		window.removeEventListener("apply-filter");
	}
	render() {
		return (
			<div>
				{this.state.items.map(item => {
					return (
						<div key={item.id} className="row">
							<div className="col-md-2">
								<div className="media-left media-middle">
									{item.eventType}
								</div>
							</div>
							<div className="col-md-10">
								<code>{JSON.stringify(item.info)}</code>
								<br />
								<span className="badge">{item.externalId}</span> {item.createdAt}
							</div>
						</div>
					);
				})}
			</div>
		);
	}
	requestEvents(range) {
		var deviceId = $(".dashboard-content").data("deviceId"),
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
}

module.exports = Events;