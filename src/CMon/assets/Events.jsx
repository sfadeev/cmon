var React = require("react"),
	request = require('superagent');

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
			<div className="table-responsive">
				<table className="table table-bordered table-condensed table-hover">
					<thead>
						<tr>
							<th>createdAt</th>
							<th>eventType</th>
							<th>externalId</th>
							<th>info</th>
						</tr>
					</thead>
					<tbody>
						{this.state.items.map(item => {
							return (
								<tr key={item.id}>
									<td>{item.createdAt}</td>
									<td>{item.eventType}</td>
									<td>{item.externalId}</td>
									<td><code>{JSON.stringify(item.info)}</code></td>
								</tr>
							);
						}) }
					</tbody>
				</table>
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