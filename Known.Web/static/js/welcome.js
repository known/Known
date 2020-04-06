layui.define(function (exports) {
	layui.use(['carousel'], function () {
		var $ = layui.jquery,
			carousel = layui.carousel,
			device = layui.device();

		$('.layadmin-carousel').each(function () {
			var a = $(this);
			carousel.render({
				elem: this,
				width: '100%',
				arrow: 'none',
				interval: a.data('interval'),
				autoplay: a.data('autoplay') === !0,
				trigger: device.ios || device.android ? 'click' : 'hover',
				anim: a.data('anim')
			})
		});
	});

	exports('welcome', {});
});