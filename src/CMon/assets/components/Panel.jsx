import React from "react";

module.exports = class Panel extends React.Component {
	render() {
		return (
			<div className="panel panel-default">
				<div className="panel-body">
					{this.props.children}
				</div>
			</div>
		);
	}
}
