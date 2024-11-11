//questionnaire.js
// 保存當前表單數據到暫存資料表
function saveFormDataToTemporary() {
    const formData = $('#questionnaireForm').serializeArray();
    formData.forEach(item => {
        $.post('/Temporary/SaveAnswer', {
            userId: $('#UserID').val(), // 用戶 ID，可能需要從隱藏欄位或其他地方獲取
            questionnaireType: $('#QuestionnaireType').val(), // 動態設置當前問卷類型
            questionId: item.name.replace(/[^\d]/g, ''), // 假設 name 包含 questionId
            answer: item.value
        });
    });
}

// 加載暫存的表單數據
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

// 切換頁籤並加載對應的暫存數據
function openTab(evt, tabName, url) {
    saveFormDataToTemporary(); // 在切換頁籤時自動保存當前表單數據

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

            // 加載暫存的數據到頁面
            loadFormDataFromTemporary(safeTabName); // 使用 safeTabName 作為 questionnaireType
        }).fail(function (xhr) {
            if (xhr.status === 403 || xhr.status === 401) {
                alert('只有防護員可以訪問此頁面。');
            } else {
                alert('載入頁面時發生錯誤，請稍後再試。');
            }
        });
    } else {
        $('#' + safeTabName).show();
        loadFormDataFromTemporary(safeTabName); // 若頁籤已加載過，直接加載暫存數據
    }

    if (evt) evt.preventDefault();
}