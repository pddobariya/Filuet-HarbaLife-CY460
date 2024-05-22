$(() => {
	// Ширина окна для ресайза
	WW = $(window).width()


	// Основной слайдер на главной
	if ($('.main_slider .swiper-container').length) {
		new Swiper('.main_slider .swiper-container', {
			loop: true,
			speed: 750,
			watchSlidesVisibility: true,
			slideActiveClass: 'active',
			slideVisibleClass: 'visible',
			spaceBetween: 24,
			slidesPerView: 1,
			navigation: {
				nextEl: '.swiper-button-next',
				prevEl: '.swiper-button-prev'
			},
			pagination: {
				el: '.swiper-pagination',
				type: 'bullets',
				clickable: true,
				bulletActiveClass: 'active'
			}
		})
	}


	// Товары
	if ($('.products .swiper-container').length) {
		if ($(window).width() > 1023) {
			productsSliderInit()
		} else {
			$('.products .swiper-wrapper').each(function () {
				productHeight($(this), parseInt($(this).css('--products_count')))
			})
		}
	}


	// Программы
	if ($('.programs .swiper-container').length) {
		if ($(window).width() > 1023) {
			programsSliderInit()
		}
	}


	// Новости
	if ($('.articles .swiper-container').length) {
		if ($(window).width() > 1023) {
			articlesSliderInit()
		}
	}


	// Поиск
	$('body').on('keydown', '.search form .input', function () {
		let _self = $(this),
			parent = $(this).closest('form')

		setTimeout(() => {
			_self.val().length
				? _self.addClass('active')
				: _self.removeClass('active')

			_self.val().length > 2
				? parent.find('.tips').fadeIn(300)
				: parent.find('.tips').fadeOut(200)
		})
	})

	$('.search form .clear_btn').click((e) => {
		e.preventDefault()

		$('.search form .input').val('').removeClass('active')
	})


	// Сюрприз
	if ($('#surprise_modal').length) {
		Fancybox.show([{
			src: '#surprise_modal',
			type: 'inline'
		}])
	}


	// Товар в корзину
	//$('.buy .buy_btn').click(function (e) {
	//	e.preventDefault()

	//	$(this).addClass('active')
	//})


	// Изменение вида отображения товаров в категории
	$('.grid_list_head .views .btn').click(function (e) {
		e.preventDefault()

		$('.grid_list_head .views .btn').removeClass('active')
		$(this).addClass('active')

		if ($(this).hasClass('grid_btn')) {
			$('.products > .list').addClass('row')
			$('.products > .list').removeClass('list')
		}

		if ($(this).hasClass('list_btn')) {
			$('.products > .row').addClass('list')
			$('.products > .row').removeClass('row')
		}
	})

	if ($(window).width() < 768) {
		$('.products:not(.not_transform) > .list').addClass('row')
		$('.products:not(.not_transform) > .list').removeClass('list')

		$('.grid_list_head .views .btn').removeClass('active')
		$('.grid_list_head .views .btn.grid_btn').addClass('active')
	}


	// Фильтр
	$priceRange = $('.filter #price_range').ionRangeSlider({
		type: 'double',
		min: 0,
		max: 289,
		from: 56,
		to: 289,
		step: 1,
		onChange: data => {
			$('.filter .price_range input.from').val(data.from.toLocaleString())
			$('.filter .price_range input.to').val(data.to.toLocaleString())

			$('.filter_selected .price .from').text(data.from.toLocaleString())
			$('.filter_selected .price .to').text(data.to.toLocaleString())
		}
	}).data("ionRangeSlider")

	$('.filter .price_range .input').keyup(function () {
		$priceRange.update({
			from: parseFloat($('.filter .price_range input.from').val().replace(/\s+/g, '')),
			to: parseFloat($('.filter .price_range input.to').val().replace(/\s+/g, ''))
		})

		$('.filter_selected .price .from').text(parseFloat($('.filter .price_range input.from').val().replace(/\s+/g, '')))
		$('.filter_selected .price .to').text(parseFloat($('.filter .price_range input.to').val().replace(/\s+/g, '')))
	})

	$('.filter_selected .price .delete_btn').click(function () {
		$priceRange.reset()

		$('.filter .price_range input.from').val($priceRange.old_from.toLocaleString())
		$('.filter .price_range input.to').val($priceRange.old_to.toLocaleString())

		$('.filter_selected .price .from').text($priceRange.old_from.toLocaleString())
		$('.filter_selected .price .to').text($priceRange.old_to.toLocaleString())
	})


	// Расширенный поиск
	$('.search_info .form .advanced_search label').click(() => {
		$('.search_info .form .hide').slideToggle(300)
	})


	// Личный кабинет - Добавление телефона/адреса
	$('.lk_info .form .add_btn').click(function (e) {
		e.preventDefault()

		let parent = $(this).closest('.section'),
			html = parent.find('.template').html()

		parent.find('.template').before(html)
	})


	// Личный кабинет - Удаление телефона/адреса
	$('body').on('click', '.lk_info .form .delete_btn', function (e) {
		e.preventDefault()

		$(this).closest('.line').remove()
	})


	// Аккордион
	$('body').on('click', '.accordion .accordion_item .head', function (e) {
		e.preventDefault()

		const $item = $(this).closest('.accordion_item'),
			$accordion = $(this).closest('.accordion')

		if ($item.hasClass('active')) {
			$item.removeClass('active').find('.data').slideUp(300)
		} else {
			$accordion.find('.accordion_item').removeClass('active')
			$accordion.find('.data').slideUp(300)

			$item.addClass('active').find('.data').slideDown(300)
		}
	})


	// Корзина
	$('.cart_info .btns .btn.checkout_btn').click(function (e) {
		if ($('.cart_info .totals #month').val() == 0) {
			e.preventDefault()

			Fancybox.show([{
				src: '#month_selected_modal',
				type: 'inline'
			}])			
		}
	})


	// Оформление заказа
	//$('.fixed_checkout_panel .next_btn, .checkout_info .btns .btn.next_btn').click(function (e) {
	//	e.preventDefault()

	//	Fancybox.show([{
	//		src: '#unable_deliver_modal',
	//		type: 'inline'
	//	}])
	//})


	// Товары в заказе
	$('.order_products .totals .spoler_btn').click(function (e) {
		e.preventDefault()

		$(this).toggleClass('active')
		$('.order_products .totals_data').slideToggle(300)
	})


	// Моб. меню
	$('header .mob_bottom .menu_btn').click(e => {
		e.preventDefault()

		$('body').addClass('menu_open')
		$('.mob_menu').addClass('show')
		$('.overlay').fadeIn(300)
	})

	$('.mob_menu .close_btn, .overlay').click(e => {
		e.preventDefault()

		$('body').removeClass('menu_open')
		$('.mob_menu').removeClass('show')
		$('.overlay').fadeOut(200)
	})


	// Моб. аккаунт
	$('header .account .mob_user').click(e => {
		e.preventDefault()

		$('body').addClass('menu_open')
		$('.mob_account').addClass('show')
		$('.overlay').fadeIn(300)
	})

	$('.mob_account .close_btn, .overlay').click(e => {
		e.preventDefault()

		$('body').removeClass('menu_open')
		$('.mob_account').removeClass('show')
		$('.overlay').fadeOut(200)
	})


	// Моб. фильтр
	$('.mob_filter_btn').click(e => {
		e.preventDefault()

		$('.mob_filter').addClass('show')
		$('.overlay').fadeIn(300)
	})

	$('.mob_filter .close_btn, .overlay').click(e => {
		e.preventDefault()

		$('.mob_filter').removeClass('show')
		$('.overlay').fadeOut(200)
	})


	// Моб. меню - Подменю
	$('.mob_menu .menu .item > button.sub_btn').click(function (e) {
		e.preventDefault()

		$('.mob_menu .menu .item > a, .mob_menu .menu .item > button').removeClass('active')

		!$(this).hasClass('active')
			? $(this).addClass('active').next().slideToggle(300)
			: $(this).next().slideToggle(200)
	})

	$('.mob_menu .menu .sub_menu a.sub_link').click(function (e) {
		e.preventDefault()

		!$(this).hasClass('active')
			? $(this).addClass('active').next().slideToggle(300)
			: $(this).removeClass('active').next().slideToggle(200)
	})


	//// Страница товара
	//if ($('.product_info .images').length) {
	//	const productThumbs = new Swiper('.product_info .thumbs .swiper-container', {
	//		loop: false,
	//		speed: 500,
	//		watchSlidesVisibility: true,
	//		slideActiveClass: 'active',
	//		slideVisibleClass: 'visible',
	//		spaceBetween: 12,
	//		slidesPerView: 'auto',
	//		direction: 'vertical'
	//	})

	//	new Swiper('.product_info .big .swiper-container', {
	//		loop: false,
	//		speed: 500,
	//		watchSlidesVisibility: true,
	//		slideActiveClass: 'active',
	//		slideVisibleClass: 'visible',
	//		spaceBetween: 20,
	//		slidesPerView: 1,
	//		thumbs: {
	//			swiper: productThumbs
	//		},
	//		navigation: {
	//			nextEl: '.swiper-button-next',
	//			prevEl: '.swiper-button-prev'
	//		}
	//	})
	//}

	
	// Страница товара - вкусы
	const tastesSliders = []

	$('.tastes .swiper-container').each(function (i) {
		$(this).addClass('tastes_s' + i)

		let slides = $(this).find('.slide').length,
			options = {
				loop: false,
				speed: 500,
				simulateTouch: false,
				allowTouchMove: true,
				noSwiping: true,
				spaceBetween: 16,
				watchSlidesVisibility: true,
				slideActiveClass: 'active',
				slideVisibleClass: 'visible',
				navigation: {
					nextEl: '.swiper-button-next',
					prevEl: '.swiper-button-prev'
				},
				breakpoints: {
					0: {
						slidesPerView: 2
					},
					768: {
						slidesPerView: 3
					},
					1024: {
						slidesPerView: 5
					},
					1280: {
						slidesPerView: 6
					},
					1440: {
						slidesPerView: 7
					}
				}
			}

		tastesSliders.push(new Swiper('.tastes_s' + i, options))

		if (slides > tastesSliders[i].params.slidesPerView) {
			options.loop = true
			options.simulateTouch = true
			options.allowTouchMove = true
			options.noSwiping = false

			tastesSliders[i].destroy(true, true)
			tastesSliders[i] = new Swiper('.tastes_s' + i, options)
		}
	})
})



$(window).on('load', () => {
	// Выравнивание элементов в сетке
	$('.category_info .sub_categories .row').each(function () {
		subСategoriesHeight($(this), parseInt($(this).css('--subСategories_count')))
	})


	// Страница товара
	productHeadCalc()


	// Фикс. шапка
	headerInit = true,
		headerHeight = $('header').outerHeight()

	$('header').wrap('<div class="header_wrap"></div>')
	$('.header_wrap').height(headerHeight)

	headerInit && $(window).scrollTop() > headerHeight
		? $('header').addClass('fixed')
		: $('header').removeClass('fixed')
})



$(window).on('resize', () => {
	if (WW != $(window).width()) {
		setTimeout(() => {
			// Фикс. шапка
			headerInit = false
			$('.header_wrap').height('auto')

			setTimeout(() => {
				headerInit = true
				headerHeight = $('header').outerHeight()

				$('.header_wrap').height(headerHeight)

				headerInit && $(window).scrollTop() > headerHeight
					? $('header').addClass('fixed')
					: $('header').removeClass('fixed')
			}, 100)


			// Моб. версия
			if (!fiestResize) {
				$('meta[name=viewport]').attr('content', 'width=device-width, initial-scale=1, maximum-scale=1')
				if ($(window).width() < 480) $('meta[name=viewport]').attr('content', 'width=480, user-scalable=no')

				fiestResize = true
			} else {
				fiestResize = false
			}


			// Товары
			if ($('.products .swiper-container').length) {
				if ($(window).width() > 1023) {
					productsSliderInit()
				} else {
					productsSliderDestroy()

					$('.products .swiper-wrapper').each(function () {
						productHeight($(this), parseInt($(this).css('--products_count')))
					})
				}
			}

			// Программы
			if ($('.programs .swiper-container').length) {
				$(window).width() > 1023
					? programsSliderInit()
					: programsSliderDestroy()
			}

			// Новости
			if ($('.articles .swiper-container').length) {
				$(window).width() > 1023
					? articlesSliderInit()
					: articlesSliderDestroy()
			}


			// Выравнивание элементов в сетке
			$('.category_info .sub_categories .row').each(function () {
				subСategoriesHeight($(this), parseInt($(this).css('--subСategories_count')))
			})


			// Изменение вида отображения товаров в категории
			if ($(window).width() < 768) {
				$('.products:not(.not_transform) > .list').addClass('row')
				$('.products:not(.not_transform) > .list').removeClass('list')

				$('.grid_list_head .views .btn').removeClass('active')
				$('.grid_list_head .views .btn.grid_btn').addClass('active')
			}


			// Страница товара
			productHeadCalc()
		}, 500)


		// Перезапись ширины окна
		WW = $(window).width()
	}
})



$(window).scroll(() => {
	// Фикс. шапка
	typeof headerInit !== 'undefined' && headerInit && $(window).scrollTop() > headerHeight
		? $('header').addClass('fixed')
		: $('header').removeClass('fixed')
})



// Выравнивание товаров
function productHeight(context, step) {
	let start = 0,
		finish = step,
		$products = context.find('.product')

	$products.find('.info').height('auto')

	$products.each(function () {
		setHeight($products.slice(start, finish).find('.info'))

		start = start + step
		finish = finish + step
	})
}


// Выравнивание подкатегорий
function subСategoriesHeight(context, step) {
	let start = 0,
		finish = step,
		$subСategories = context.find('.category')

	$subСategories.find('.name').height('auto')

	$subСategories.each(function () {
		setHeight($subСategories.slice(start, finish).find('.name'))

		start = start + step
		finish = finish + step
	})
}



// Товары
const productsSliderInit = () => {
	productsSliders = []

	$('.products .swiper-container').each(function (i) {
		let slides = $(this).find('.slide').length,
			this_ID = $(this).attr('id'),
			options = {
				loop: false,
				speed: 500,
				simulateTouch: false,
				allowTouchMove: false,
				noSwiping: true,
				watchSlidesVisibility: true,
				slideActiveClass: 'active',
				slideVisibleClass: 'visible',
				navigation: {
					nextEl: '.swiper-button-next',
					prevEl: '.swiper-button-prev'
				},
				breakpoints: {
					0: {
						spaceBetween: 24,
						slidesPerView: 1
					},
					768: {
						spaceBetween: 24,
						slidesPerView: 2
					},
					1024: {
						spaceBetween: 24,
						slidesPerView: 3
					},
					1280: {
						spaceBetween: 24,
						slidesPerView: 4
					}
				},
				on: {
					init: swiper => {
						setTimeout(() => {
							productHeight($(swiper.$el), $(swiper.$el).find('.product').length)
						})
					},
					resize: swiper => {
						setTimeout(() => {
							productHeight($(swiper.$el), $(swiper.$el).find('.product').length)
						})
					}
				}
			}

		productsSliders[i] = new Swiper('#' + this_ID, options)

		if (slides > productsSliders[i]?.params?.slidesPerView) {
			options.loop = true
			options.simulateTouch = true
			options.allowTouchMove = true
			options.noSwiping = false

			productsSliders[i].destroy(true, true)
			productsSliders[i] = new Swiper('#' + this_ID, options)
		}
	})
}

const productsSliderDestroy = () => {
	if (typeof productsSliders !== 'undefined') {
		productsSliders.map((val, i) => {
			productsSliders[i].destroy(true, true)
			$('.products .swiper-wrapper, .products .swiper-slide').removeAttr('style')
		})

		productsSliders = []
	}
}



// Программы
const programsSliderInit = () => {
	programsSliders = []

	$('.programs .swiper-container').each(function (i) {
		let slides = $(this).find('.slide').length,
			this_ID = $(this).attr('id'),
			options = {
				loop: false,
				speed: 500,
				simulateTouch: false,
				allowTouchMove: false,
				noSwiping: true,
				watchSlidesVisibility: true,
				slideActiveClass: 'active',
				slideVisibleClass: 'visible',
				navigation: {
					nextEl: '.swiper-button-next',
					prevEl: '.swiper-button-prev'
				},
				breakpoints: {
					0: {
						spaceBetween: 24,
						slidesPerView: 1
					},
					768: {
						spaceBetween: 24,
						slidesPerView: 2
					},
					1024: {
						spaceBetween: 24,
						slidesPerView: 3
					},
					1280: {
						spaceBetween: 24,
						slidesPerView: 4
					}
				},
				on: {
					init: swiper => {
						setTimeout(() => {
							setHeight($(swiper.$el).find('.program'))
						})
					},
					resize: swiper => {
						setTimeout(() => {
							setHeight($(swiper.$el).find('.program'))
						})
					}
				}
			}

		programsSliders[i] = new Swiper('#' + this_ID, options)

		if (slides > programsSliders[i].params.slidesPerView) {
			options.loop = true
			options.simulateTouch = true
			options.allowTouchMove = true
			options.noSwiping = false

			programsSliders[i].destroy(true, true)
			programsSliders[i] = new Swiper('#' + this_ID, options)
		}
	})
}

const programsSliderDestroy = () => {
	if (typeof programsSliders !== 'undefined') {
		programsSliders.map((val, i) => {
			programsSliders[i].destroy(true, true)
			$('.programs .swiper-wrapper, .programs .swiper-slide').removeAttr('style')
		})

		programsSliders = []
	}
}



// Новости
const articlesSliderInit = () => {
	$('.articles .swiper-container').each(function (i) {
		articlesSlider = new Swiper('.articles .swiper-container', {
			loop: true,
			speed: 500,
			watchSlidesVisibility: true,
			slideActiveClass: 'active',
			slideVisibleClass: 'visible',
			spaceBetween: 24,
			navigation: {
				nextEl: '.swiper-button-next',
				prevEl: '.swiper-button-prev'
			},
			breakpoints: {
				0: {
					slidesPerView: 1
				},
				768: {
					slidesPerView: 2
				},
				1024: {
					slidesPerView: 3
				},
				1280: {
					slidesPerView: 4
				}
			}
		})
	})
}

const articlesSliderDestroy = () => {
	if (typeof articlesSlider !== 'undefined') {
		articlesSlider.destroy(true, true)
		$('.articles .swiper-wrapper, .articles .swiper-slide').removeAttr('style')
	}
}



// Страница товара
const productHeadCalc = () => {
	let productHeadHeight = 0

	$(window).width() < 1024
		? productHeadHeight = $('.product_info .product_data .mob_head').outerHeight()
		: productHeadHeight = 0

	$('.product_info .product_data .data .col_right.mob_top').css('top', productHeadHeight)
	$('.product_info .product_data').css('padding-top', productHeadHeight)
}