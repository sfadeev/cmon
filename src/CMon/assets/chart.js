var d3 = require("d3");

var chart = d3.select('#chart');

var formatDateTime = d3.timeFormat("%x %X"),
	formatTime = d3.timeFormat("%X"),
	formatValue = d3.format(",.2f"),
	bisectDate = d3.bisector(function(d) { return d.period; }).left;

// var parseTime = d3.timeParse("%Y-%m-%dT%H:%M:%S");

var svg, g,
	x, y, z,
	width, 	height;

var line = d3.line()
	.curve(d3.curveMonotoneX)
	// .defined(function (d) { return d.avg; }) // https://github.com/d3/d3-shape/blob/master/README.md#line_defined
	.x(function(d) { return x(d.period); })
	.y(function(d) { return y(d.avg); });

var area = d3.area()
	.curve(d3.curveMonotoneX)
	.x(function(d) { return x(d.period); })
	.y0(function(d) { return y(d.min); })
	.y1(function(d) { return y(d.max); });

var options = {
	container: { width: 600, minHeight: 300, maxHeight: 400 },
	margin: { top: 20, right: 60, bottom: 20, left: 20 },
	ticks: { x: 24, y: 12 },
	showZero: true,
	hours: 1
};

var init = function() {

	options.container.width = parseInt(chart.style('width'), 10);
	options.container.height =
		Math.min(
			Math.max(
				document.documentElement.clientHeight - 200,
				options.container.minHeight),
			options.container.maxHeight);

	options.ticks.x = options.container.width / 60;

	chart.select('svg')
		.remove();

	svg = chart.append("svg")
		.attr("width", options.container.width + "px")
		.attr("height", options.container.height + "px");

	g = svg.append("g")
		.attr("transform", "translate(" + options.margin.left + "," + options.margin.top + ")");

	width = options.container.width - options.margin.left - options.margin.right;
	height = options.container.height - options.margin.top - options.margin.bottom;

	x = d3.scaleTime().range([0, width]);
	y = d3.scaleLinear().range([height, 0]);
	z = d3.scaleOrdinal(d3.schemeCategory10);

}

var parse = function (data) {

	data.beginDate = d3.isoParse(data.beginDate);
	data.endDate = d3.isoParse(data.endDate);

	for (var i = 0; i < data.inputs.length; i++) {
		for (var j = 0; j < data.inputs[i].values.length; j++) {
			var value = data.inputs[i].values[j];
			value.period = d3.isoParse(value.period);
		}
	}

	return data;
}

var draw = function(data, update) {

	var x_domain = [data.beginDate, data.endDate];
	// var x_domain = d3.extent(data.inputs[0].values, function (d) { return d.period; });

	var y_domain = [
		d3.min(data.inputs, function(i) { return d3.min(i.values, function(d) { return d.min; }) }) * 0.75 || 0.0,
		d3.max(data.inputs, function(i) { return d3.max(i.values, function(d) { return d.max; }) }) * 1.10 || 0.0
	];

	if (options.showZero) {
		y_domain[0] = Math.min(y_domain[0], 0.0);
		y_domain[1] = Math.max(y_domain[1], 0.0);
	}

	var z_domain = data.inputs.map(function(c) { return c.inputNo; });

	x = x.domain(x_domain);
	y = y.domain(y_domain);
	z = z.domain(z_domain);

	var mousemove = function() {
		var x0 = x.invert(d3.mouse(this)[0]);

		g.selectAll(".area").data().forEach(function(input, i) {

			var valueIndex = bisectDate(input.values, x0, 1),
				d0 = input.values[valueIndex - 1],
				d1 = input.values[valueIndex] || d0,
				d = x0 - d0.period > d1.period - x0 ? d1 : d0;

			g.select(".focus-line")
				.attr("transform", "translate(" + x(d.period) + ", 0)")
				.select("text").html(formatDateTime(d.period));

			g.select(".focus-" + input.inputNo)
				.attr("transform", "translate(" + x(d.period) + "," + y(d.avg) + ")")
				.select("text").text(formatValue(d.avg));
		});
	};

	if (update) {
		var t = svg.transition().duration(300);

		// x axis
		t.select(".axis--x")
			.attr("transform", "translate(0," + height + ")")
			.call(d3.axisBottom(x).ticks(options.ticks.x));

		// vertical lines
		g.selectAll("g.axis--x g.tick line.tick-ext")
			.remove();
		g.selectAll("g.axis--x g.tick")
			.append("line")
			.attr("class", "tick-ext")
			.attr("x1", 0.5)
			.attr("y1", 0)
			.attr("x2", 0.5)
			.attr("y2", -height);

		// y axis
		t.select(".axis--y")
			.call(d3.axisLeft(y).ticks(options.ticks.y));

		// horizontal lines
		g.selectAll("g.axis--y g.tick line.tick-ext")
			.remove();
		g.selectAll("g.axis--y g.tick")
			.append("line")
			.attr("class", "tick-ext")
			.attr("x1", 0)
			.attr("y1", 0.5)
			.attr("x2", width)
			.attr("y2", 0.5);

		// inputs
		g.selectAll(".input")
			.data(data.inputs)
			.enter().append("g")
			.attr("class", "input");

		// change the area
		g.selectAll(".area")
			.data(data.inputs)
			.attr("d", function(d) { return area(d.values); })
			.style("fill", function(d) { return z(d.inputNo); })
			.transition().duration(100);

		// change the line
		g.selectAll(".line")
			.data(data.inputs)
			.attr("d", function(d) { return line(d.values); })
			.style("stroke", function(d) { return z(d.inputNo); })
			.transition().duration(100);

	} else {

		// x axis
		g.append("g")
			.attr("class", "axis axis--x")
			.attr("transform", "translate(0," + height + ")")
			.call(d3.axisBottom(x).ticks(options.ticks.x));

		// vertical lines
		g.selectAll("g.axis--x g.tick")
			.append("line")
			.attr("class", "tick-ext")
			.attr("x1", 0.5)
			.attr("y1", 0)
			.attr("x2", 0.5)
			.attr("y2", -height);

		// y axis
		g.append("g")
			.attr("class", "axis axis--y")
			.call(d3.axisLeft(y).ticks(options.ticks.y))
			.append("text")
			.attr("transform", "rotate(-90)")
			.attr("y", 7)
			.attr("dy", "0.71em")
			.attr("fill", "#999")
			.text("Temperature, ºC");

		// horizontal lines
		g.selectAll("g.axis--y g.tick")
			.append("line")
			.attr("class", "tick-ext")
			.attr("x1", 0)
			.attr("y1", 0.5)
			.attr("x2", width)
			.attr("y2", 0.5);

		var gs = g.selectAll(".input")
			.data(data.inputs)
			.enter().append("g")
			.attr("class", "input");

		gs.append("path")
			.attr("class", "area")
			.attr("d", function(d) { return area(d.values); })
			.style("fill", function(d) { return z(d.inputNo); });

		gs.append("path")
			.attr("class", "line")
			.attr("d", function(d) { return line(d.values); })
			.style("stroke", function(d) { return z(d.inputNo); });

		gs.append("text")
			.datum(function(d) { return { inputNo: d.inputNo, name: d.name, value: d.values[d.values.length - 1] }; })
			.attr("transform", function(d) { return "translate(" + x(d.value.period) + "," + y(d.value.avg) + ")"; })
			.attr("x", 5.0)
			.attr("dy", "0.3em")
			.style("font", "10px monospace")
			.style("fill", function(d) { return z(d.inputNo); })
			.text(function(d) { return d.name; });

		// focus line
		var focuslLine = g.append("g")
			.attr("class", "focus-line")
			.style("display", "none");

		focuslLine.append("line")
			.attr("x1", 0)
			.attr("y1", 0)
			.attr("x2", 0)
			.attr("y2", height);

		focuslLine.append("text")
			.attr("dx", 7);

		// focus circles
		var focus = g.selectAll(".focus")
			.data(data.inputs)
			.enter().append("g")
			.attr("class", function(d) { return "focus focus-" + d.inputNo; })
			.style("display", "none");

		focus.append("circle")
			.attr("r", 3.5)
			.style("stroke", function(d) { return z(d.inputNo); });

		focus.append("text")
			.attr("x", 7)
			.attr("dy", "-0.75em");

		// overlay
		g.append("rect")
			.attr("class", "overlay")
			.attr("width", width)
			.attr("height", height)
			.on("mouseover",
				function() {
					focuslLine.style("display", null);
					focus.style("display", null);
				})
			.on("mouseout",
				function() {
					focuslLine.style("display", "none");
					focus.style("display", "none");
				})
			.on("mousemove", mousemove);
	}
};

function getDataUrl() {
	return "/api/values?deviceId=" + chart.attr("data-device-id") + "&from=" + $("#from").val() + "&to=" + $("#to").val() + "&t=" + new Date().getTime();
}

var toid;

var loadData = function (update) {

	if (toid) {
		clearTimeout(toid);
		toid = null;
	}

	$(".loading-indicator").show();

	d3.json(getDataUrl(),
		function (error, data) {
			
			if (error) return; // throw error; // todo: log error

			options.data = parse(data);
			draw(data, update);

			$(".loading-indicator").hide();

			if ($("#to").val() === "now") {
				toid = setTimeout(function () {
					loadData(true);
				}, 15000);
			}
		}
	);
}

init();
loadData(false);

$(".btn-group .btn").click(function () {
	$("#from").val("now-" + $(this).text());
	$("#to").val("now");
	loadData(true);
});

$(".input-group input").keypress(function (e) {
	if(e.which === 13) loadData(true);
});

$(".input-group .btn").click(function () {
	loadData(true);
});

$(window).resize(function () {
	init();
	draw(options.data, false);
});

$(".sidebar").on("toggle", function () {
	init();
	draw(options.data, false);
});
