!function ($) {
	$(function () {

		var $window = $(window);
		var $body = $("body");
		var $sidebar = $(".sidebar");

		var resizeSidebar = function () {
			if ($window.width() < (768 - 20) || localStorage.getItem("sidebar-collapsed")) {
				$body.addClass("sidebar-collapsed");
				$sidebar.trigger("toggle");
			} else if (!localStorage.getItem("sidebar-collapsed")) {
				$body.removeClass("sidebar-collapsed");
				$sidebar.trigger("toggle");
			}
		}

		$window.resize(resizeSidebar);

		$sidebar.find(".sidebar-toggle").click(function() {
			$body.toggleClass("sidebar-collapsed");
			if ($body.hasClass("sidebar-collapsed"))
				localStorage.setItem("sidebar-collapsed", true);
			else
				localStorage.removeItem("sidebar-collapsed");
			$sidebar.trigger("toggle");
		});

		resizeSidebar();
	});
}(jQuery)