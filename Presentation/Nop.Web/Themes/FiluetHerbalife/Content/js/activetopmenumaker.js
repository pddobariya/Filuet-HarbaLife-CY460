$(document).ready(function () {
    const $homepage = $('.home-page'),
        $pagehead = $('.page_head');

    if ($homepage.length) {
        MakeActive($('.home-page-menu-item > a'));
    } else if ($('.page_head').length) {
        if ($pagehead.hasClass('programm')) {
            MakeActive($('.programm-page-menu-item > a'));
        }
        else if ($pagehead.hasClass('catalog')) {
            MakeActive($('.catalog-page-menu-item > a'));
        }
        else if ($pagehead.hasClass('topic')) {
            MakeActive($('.topic-page-menu-item > a'));
        }
        else if ($pagehead.hasClass('faq')) {
            MakeActive($('.faq-page-menu-item > a'));
        }
    } 
});

function MakeActive(element) {
    element.addClass('active');
}