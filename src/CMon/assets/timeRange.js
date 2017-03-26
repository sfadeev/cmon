var $range = $("#time-range"),
	$form = $range.find("form"),
	$from = $form.find("#from"),
	$to = $form.find("#to");

var apply = function(range) {

	localStorage.setItem("time-range", JSON.stringify(range));

	$("#selected-range-name")
		.text(range.name || (range.from + " — " + range.to));

	$from.val(range.from);
	$to.val(range.to);

	$range
		.collapse("hide")
		.trigger("apply", range);
}

$form.submit(function () {

	apply({
		from: $from.val(),
		to: $to.val()
	});

	return false;
});

$range.find("a").click(function () {
	var a = $(this);

	apply({
		name: a.data("name"),
		from: a.data("from"),
		to: a.data("to")
	});

	return false;
});

$(function () {

	var range,
		defaultRange = { name: "Last 1 hour", from: "now-1h", to: "now" }

	try {
		range = JSON.parse(localStorage.getItem("time-range"));
	} catch (e) {
		// no-op
	}

	apply(range || defaultRange);
});