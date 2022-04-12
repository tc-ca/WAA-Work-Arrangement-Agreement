// a11y improvments on top of accordion
$('.ui.accordion .title').each(function (index) {
    // put a tab stop on all .title elements
    $(this).attr('tabindex', '0');
});
// handle arrow keys in accordion
$('.ui.accordion .title').on('keydown', function (e) {
    var index = $(this).index('.title');
    // enter/space, toggle menus
    if (e.which === 13 || e.which === 32) {
        e.preventDefault();
        $(this).click();
    }

    // arrow right, open menu
    if (e.which === 39) {
        e.preventDefault();
        $(this).parent('.accordion').accordion('open', index);
    }

    // arrow left, close menu
    if (e.which === 37) {
        e.preventDefault();
        $(this).parent('.accordion').accordion('close', index);
    }

    // arrow down, next item
    if (e.which === 40) {
        e.preventDefault();
        var next = $(this).nextAll('.title, .content:visible').first();

        if (next.hasClass('content')) {
            next.find('.item').first().focus();
        }
        else {
            next.focus();
        }
    }

    // arrow up, prev item
    if (e.which === 38) {
        e.preventDefault();

        var prev = $(this).prevAll('.title, .content:visible').first();
        if (prev.hasClass('content')) {
            prev.find('.item').last().focus();
        }
        else {
            prev.focus();
        }


    }
});
// handle arrow keys in menu inside accordion
$('.ui.accordion').on('keydown', '.ui.menu .item', function (e) {
    // arrow down, next item
    if (e.which === 40) {
        e.preventDefault();
        var next = $(this).nextAll('.item').first();

        if (next.length === 0) {
            next = $(this).parents('.content').next('.title');
        }

        next.focus();
    }

    // arrow up, prev item
    if (e.which === 38) {
        e.preventDefault();
        var prev = $(this).prevAll('.item').first();

        if (prev.length === 0) {
            prev = $(this).parents('.content').prev('.title');
        }

        prev.focus();
    }

    // arrow left, close parent accordion
    if (e.which === 37) {
        e.preventDefault();

        var title = $(this).parents('.content').prev('.title');
        console.log('closing', $(this).parents('.accordion').first(), title.index('.title'));
        $(this).parents('.accordion').first().accordion('close', title.index('.title'))
        title.focus();
    }

});