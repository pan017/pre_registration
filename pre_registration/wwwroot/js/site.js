function ShowModalWindow(header, text, isConfirmForm) {
    $.get('/home/ShowModalWindow?header=' + header + '&text=' + text + '&isConfirmForm=' + isConfirmForm, function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

function sendAjaxForm(ajax_form, url) {
    var result;
    jQuery.ajax({
        url: url, //url страницы (action_ajax_form.php)
        type: "POST", //метод отправки
        dataType: "html", //формат данных
        data: jQuery("#" + ajax_form).serialize(),  // Сеарилизуем объект
        success: function (response) { //Данные отправлены успешно
            result = response;
        },
        error: function (response) { // Данные не отправлены
            result = "Ошибка. Данные не отправленны.";
        }       
    });
    return result;
}