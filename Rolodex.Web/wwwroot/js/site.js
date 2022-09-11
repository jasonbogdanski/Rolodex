// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var highlightFields = function (response) {
    $.each(response, function (propName, val) {
        var nameSelector = '[name = "' + propName.replace(/(:|\.|\[|\])/g, "\\$1") + '"]',
            idSelector = '#' + propName.replace(/(:|\.|\[|\])/g, "\\$1");
        var $el = $(nameSelector) || $(idSelector);

        if (val.Errors.length > 0) {
            $.each(val.Errors, function (index, error) {
                $el.addClass('is-invalid');
                const $errorMessage = $('<div></div>').addClass('invalid-feedback').text(error.ErrorMessage);
                const $wrapper = $el.closest('.form-group');
                $errorMessage.appendTo($wrapper);
            })
        } else {
            $el.removeClass('is-invalid');
        }
    });
};

var clearErrors = function () {
    $('.invalid-feedback').remove();
};

var highlightErrors = function (xhr) {
    try {
        var data = JSON.parse(xhr.responseText);
        clearErrors();
        highlightFields(data);
        window.scrollTo(0, 0);
    } catch (e) {
        alert(e);
    }
};

var redirect = function (data) {
    if (data.redirect) {
        window.location = data.redirect;
    } else {
        window.scrollTo(0, 0);
        window.location.reload();
    }
};

$('form[method=post]').not('.no-ajax').on('submit', function () {
    var submitBtn = $(this).find('[type="submit"]');

    submitBtn.prop('disabled', true);
    $(window).unbind();

    var $this = $(this),
        formData = $this.serialize();

    $this.find('div').removeClass('is-invalid');

    $.ajax({
        url: $this.attr('action'),
        type: 'post',
        data: formData,
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        dataType: 'json',
        statusCode: {
            200: redirect
        },
        complete: function () {
            submitBtn.prop('disabled', false);
        }
    }).fail(highlightErrors);

    return false;
});