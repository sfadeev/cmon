import request from "superagent";

var xsrfToken = document.querySelector("input[name=__RequestVerificationToken]").value;

export function post(url, args, onResult) {

	console.log("Fetching", url, args);

	return request
		.post(url)
		.set("X-XSRF-TOKEN", xsrfToken)
		.query(args)
		.end((err, res) => {

			console.log("Fetching result", res.body);

			if (onResult) {
				onResult(err, res);
			}

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