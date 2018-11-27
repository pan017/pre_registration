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

function checkNumberValue(input) {
    var value = input.value;
    var rep = /[-\./,/|~;@@!#$%_+=^&**()<>":'a-zA-Zа-яА-Я]/;
    if (rep.test(value)) {
        value = value.replace(rep, '');
        input.value = value;
    }
}

function ValidMail() {
    var re = '/^[\w-\.]+@@[\w-]+\.[a-z]{2,4}$/i';
    var myMail = document.getElementById('Client_UserData_EmailAdress').value;
    var valid = re.test(myMail);
    if (valid) output = '';
    else {
        output = 'Пожалуйста, введите существующий адрес электронной почты<br>так как на него будет выслан ответ службы "одно окно"';
    }
    document.getElementById('Client_UserData_EmailAdress_validation').innerHTML = output;
    return valid;
}   