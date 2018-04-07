import React from "react";
import classNames from "classnames";

module.exports = class Pagination extends React.Component {
	constructor(props) {
		super(props);
		this.state = {
			pageNo: 1,
			pageSize: 10,
			maxPageCount: 5
		};
	}

	onClick (pageNo, event) {
		event.preventDefault();
		if (pageNo) {
			this.setState({ pageNo: pageNo });
		}
	}

	render() {
		var { totalCount } = this.props;
		var { pageNo, pageSize, maxPageCount } = this.state;

		var pageCount = Math.ceil(totalCount/pageSize),
			fromPage = Math.max(Math.round(pageNo - maxPageCount/2), 1),
			toPage = Math.min(fromPage + maxPageCount - 1, pageCount),
			prevPage = pageNo > 1 ? pageNo - 1 : null,
			nextPage = pageNo < pageCount ? pageNo + 1 : null;

		var pages = [...Array(toPage - fromPage + 1).keys()].map(x => x + fromPage);

		return (
			<nav aria-label="Page navigation">
				<ul className="pagination pagination-sm">
					<li className={ classNames({ disabled: prevPage == null }) }>
						<a href="#" aria-label="Previous" onClick={event => this.onClick(prevPage, event)}>
							<span aria-hidden="true">&laquo;</span>
						</a>
					</li>

					{ pages.map(x => {
						return (
							<li key={x} className={ classNames({ active: x == pageNo }) }>
								<a href="#" onClick={event => this.onClick(x, event)}>{x}</a>
							</li>
						)
					}) }

					<li className={ classNames({ disabled: nextPage == null }) }>
						<a href="#" aria-label="Next" onClick={event => this.onClick(nextPage, event)}>
							<span aria-hidden="true">&raquo;</span>
						</a>
					</li>
				</ul>
			</nav>
		);
	}
}
