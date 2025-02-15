document.addEventListener("DOMContentLoaded", function () {
    function validateForm() {
        let isValid = true;
        let firstErrorElement = null;
        let errorSections = new Set();

        // 清除所有錯誤樣式
        document.querySelectorAll(".error-text").forEach(el => el.classList.add("hidden"));
        document.querySelectorAll(".required-radio, .required-checkbox, .required-text").forEach(el => el.classList.remove("border-red-500"));

        // 遍歷所有問卷區塊
        document.querySelectorAll(".question-step").forEach(section => {
            const sectionTitle = section.querySelector("h3, h2")?.innerText.trim();
            let sectionHasError = false;

            // 【1】檢查 radio 必填
            section.querySelectorAll(".required-radio").forEach(radio => {
                const name = radio.getAttribute("name");
                const radios = section.querySelectorAll(`input[name="${name}"]`);
                const checked = [...radios].some(r => r.checked);

                if (!checked) {
                    isValid = false;
                    sectionHasError = true;
                    radios.forEach(r => r.classList.add("border-red-500"));
                    if (!firstErrorElement) firstErrorElement = section;
                }
            });

            // 【2】檢查 Checkbox 是否至少勾選一個
            const checkboxes = section.querySelectorAll(".required-checkbox");
            let anyChecked = [...checkboxes].some(cb => cb.checked);
            if (checkboxes.length > 0 && !anyChecked) {
                isValid = false;
                sectionHasError = true;
                checkboxes.forEach(cb => cb.classList.add("border-red-500"));
                if (!firstErrorElement) firstErrorElement = section;
            }

            // 【3】"其他" Checkbox 需要填寫對應輸入框
            section.querySelectorAll(".other-checkbox").forEach(otherCheckbox => {
                const relatedTextField = document.querySelector(otherCheckbox.dataset.target);
                if (otherCheckbox.checked && relatedTextField && relatedTextField.value.trim() === "") {
                    isValid = false;
                    sectionHasError = true;
                    relatedTextField.classList.add("border-red-500");
                    if (!firstErrorElement) firstErrorElement = section;
                }
            });

            if (sectionHasError && sectionTitle) {
                errorSections.add(sectionTitle);
            }
        });

        // 若有錯誤，顯示錯誤訊息
        if (!isValid) {
            Swal.fire({
                icon: "error",
                title: "請完成以下問卷項目",
                html: [...errorSections].map(title => `<b>${title}</b>`).join("<br>"),
                confirmButtonText: "確定"
            });

            if (firstErrorElement) {
                firstErrorElement.scrollIntoView({ behavior: "smooth", block: "center" });
            }
        }

        return isValid;
    }

    // 監聽 "其他" Checkbox 變化，顯示/隱藏對應的輸入框
    document.body.addEventListener("change", function (event) {
        const target = event.target;
        if (target.classList.contains("other-checkbox")) {
            const relatedTextField = document.querySelector(target.dataset.target);
            if (relatedTextField) {
                relatedTextField.classList.toggle("hidden", !target.checked);
            }
        }
    });

    // 提交按鈕驗證表單
    document.getElementById("submitButton").addEventListener("click", function (event) {
        if (!validateForm()) {
            event.preventDefault(); // 阻止表單提交
        }
    });
});