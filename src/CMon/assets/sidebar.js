!function ($) {
	$(function () {

		var $window = $(window);
		var $body = $("body");
		var $sidebar = $(".sidebar");

		var resizeSidebar = function () {
			if ($window.width() < (768 - 20) || localStorage.getItem("sidebar-collapsed")) {
				$body.addClass("sidebar-collapsed");
			} else if (!localStorage.getItem("sidebar-collapsed")) {
				$body.removeClass("sidebar-collapsed");
			}
		}

		$window.resize(resizeSidebar);

		$sidebar.find(".sidebar-toggle").click(function() {
			$body.toggleClass("sidebar-collapsed");
			if ($body.hasClass("sidebar-collapsed"))
				localStorage.setItem("sidebar-collapsed", true);
			else
				localStorage.removeItem("sidebar-collapsed");
		});

		resizeSidebar();
	});
}(jQuery)