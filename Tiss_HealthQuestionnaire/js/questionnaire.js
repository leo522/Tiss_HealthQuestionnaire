function saveFormDataToTemporary() {
    const formData = $('#questionnaireForm').serializeArray();
    formData.forEach(item => {
        $.post('/Temporary/SaveAnswer', {
            userId: $('#UserID').val(),
            questionnaireType: $('#QuestionnaireType').val(),
            questionId: item.name.replace(/[^\d]/g, ''),
            answer: item.value
        });
    });
}

function loadFormDataFromTemporary(questionnaireType) {
    $.get('/Temporary/LoadAnswers', { userId: $('#UserID').val(), questionnaireType: questionnaireType }, function (answers) {
        answers.forEach(answer => {
            const $input = $('[name="question_' + answer.QuestionID + '"]');
            if ($input.attr('type') === 'radio' || $input.attr('type') === 'checkbox') {
                $input.filter('[value="' + answer.Answer + '"]').prop('checked', true);
            } else {
                $input.val(answer.Answer);
            }
        });
    });
}

function openTab(evt, tabName, url) {
    saveFormDataToTemporary();

    const safeTabName = tabName.replace(/[^a-zA-Z0-9-_]/g, '');
    $('.tabcontent').hide();
    $('.tablinks').removeClass('active');
    if (evt && evt.currentTarget) {
        $(evt.currentTarget).addClass('active');
    } else {
        $(`button[onclick*="${safeTabName}"]`).addClass('active');
    }

    if ($('#' + safeTabName).length === 0) {
        $.get(url, function (data) {
            $('<div>', { id: safeTabName, class: 'tabcontent' }).html(data).appendTo('#tabContent').show();

            loadFormDataFromTemporary(safeTabName);
        }).fail(function (xhr) {
            if (xhr.status === 403 || xhr.status === 401) {
                alert('只有防護員可以訪問此頁面。');
            } else {
                alert('載入頁面時發生錯誤，請稍後再試。');
            }
        });
    } else {
        $('#' + safeTabName).show();
        loadFormDataFromTemporary(safeTabName);
    }

    if (evt) evt.preventDefault();
}