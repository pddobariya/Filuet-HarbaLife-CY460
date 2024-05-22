$(document).ready(function () {
    const $desktop_filter = $('#desktop_filter');
    const $mob_filter = $('#mob_filter .scroll');

    if ($(window).width() > 1270) {
        swap($mob_filter, $desktop_filter)
    }
    else {
        swap($desktop_filter, $mob_filter)
    }

    $(window).resize(function () {
        if ($(window).width() >= 1270) {
            swap($mob_filter, $desktop_filter)
        }
        else {
            swap($desktop_filter, $mob_filter)
        }
    });

    $(document).on('click', '#mob_filter_btn', function (e) {
        e.preventDefault();

        swap($desktop_filter, $mob_filter)
    });


});

function swap(oldelement, newelement) {
    const $childs = oldelement.children();

    if ($childs.length > 0) {
        newelement.append($childs);
        oldelement.empty();
    }
}