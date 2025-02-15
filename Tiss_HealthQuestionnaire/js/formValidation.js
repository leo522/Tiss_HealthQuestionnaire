document.addEventListener("DOMContentLoaded", function () {
    function validateForm() {
        let isValid = true;
        let firstErrorElement = null;
        let errorSections = new Set();

        // 清除錯誤樣式
        document.querySelectorAll(".error-text").forEach(el => el.classList.add("hidden"));
        document.querySelectorAll(".required-radio, .required-checkbox, .required-text, select").forEach(el => el.classList.remove("border-red-500"));

        // 遍歷所有問卷區塊
        document.querySelectorAll(".question-step").forEach(section => {
            const sectionTitle = section.querySelector("h3, h2")?.innerText.trim();
            let sectionHasError = false;

            /** 📌 1️⃣ 檢查 `radio` 是否有選擇 **/
            section.querySelectorAll(".required-radio").forEach(radio => {
                const groupName = radio.name;
                const radios = document.querySelectorAll(`input[name="${groupName}"]`);
                if (!Array.from(radios).some(r => r.checked)) {
                    isValid = false;
                    sectionHasError = true;
                    radios.forEach(r => r.classList.add("border-red-500"));
                    if (!firstErrorElement) firstErrorElement = section;
                }
            });

            /** 📌 2️⃣ 檢查 `checkbox` 是否至少勾選一個 **/
            const checkboxes = section.querySelectorAll(".required-checkbox:not(.other-checkbox)");
            if (checkboxes.length > 0) {
                let anyChecked = [...checkboxes].some(cb => cb.checked);
                if (!anyChecked) {
                    isValid = false;
                    sectionHasError = true;
                    checkboxes.forEach(cb => cb.classList.add("border-red-500"));
                    if (!firstErrorElement) firstErrorElement = section;
                }
            }

            /** 📌 3️⃣ 檢查「其他 (Others)」的輸入框 **/
            section.querySelectorAll(".other-checkbox").forEach(otherCheckbox => {
                const relatedTextField = document.querySelector(otherCheckbox.dataset.target);
                if (otherCheckbox.checked && relatedTextField && relatedTextField.value.trim() === "") {
                    isValid = false;
                    sectionHasError = true;
                    relatedTextField.classList.add("border-red-500");
                    if (!firstErrorElement) firstErrorElement = section;
                }
            });

            /** 📌 4️⃣ 檢查「藥物史」與「營養品」 **/
            if (section.dataset.step === "6" || section.dataset.step === "7") {
                let drugChecked = section.querySelectorAll('input[name^="drug_"]:checked').length;
                let otherDrugInput = section.querySelector('input[name="OtherDrug"]').value.trim();
                let supplementChecked = section.querySelectorAll('input[name^="supplement_"]:checked').length;
                let otherSupplementInput = section.querySelector('input[name="OtherSupplements"]').value.trim();

                if (section.dataset.step === "6" && drugChecked === 0 && otherDrugInput === "") {
                    isValid = false;
                    sectionHasError = true;
                    if (!firstErrorElement) firstErrorElement = section;
                }
                if (section.dataset.step === "7" && supplementChecked === 0 && otherSupplementInput === "") {
                    isValid = false;
                    sectionHasError = true;
                    if (!firstErrorElement) firstErrorElement = section;
                }
            }

            /** 📌 5️⃣ 檢查「過去傷害」與「目前傷害」 **/
            let pastInjuryYes = section.querySelector('input[name="pastInjuryStatus"][value="yes"]:checked');
            let currentInjuryYes = section.querySelector('input[name="currentInjuryStatus"][value="yes"]:checked');

            if (pastInjuryYes || currentInjuryYes) {
                // 必須選擇「受傷部位」
                let injuryChecked = section.querySelectorAll('input[name="PastInjuryLeft"]:checked, input[name="PastInjuryRight"]:checked, input[name="CurrentInjuryLeft"]:checked, input[name="CurrentInjuryRight"]:checked').length > 0;
                if (!injuryChecked) {
                    isValid = false;
                    sectionHasError = true;
                    section.querySelectorAll('input[name="PastInjuryLeft"], input[name="PastInjuryRight"], input[name="CurrentInjuryLeft"], input[name="CurrentInjuryRight"]').forEach(el => el.classList.add("border-red-500"));
                    if (!firstErrorElement) firstErrorElement = section;
                }

                // 必須選擇「傷勢類型」
                let injuryTypeSelected = section.querySelector('select[name="SelectedInjuryTypes"], select[name="SelectedCurrentInjuryTypes"]');
                if (injuryTypeSelected && injuryTypeSelected.value === "") {
                    isValid = false;
                    sectionHasError = true;
                    injuryTypeSelected.classList.add("border-red-500");
                    if (!firstErrorElement) firstErrorElement = section;
                }

                // 必須選擇「治療方式」
                let treatmentChecked = section.querySelectorAll('input[name="SelectedTreatmentMethods"]:checked, input[name="SelectedCurrentTreatmentMethods"]:checked').length > 0;
                if (!treatmentChecked) {
                    isValid = false;
                    sectionHasError = true;
                    section.querySelectorAll('input[name="SelectedTreatmentMethods"], input[name="SelectedCurrentTreatmentMethods"]').forEach(el => el.classList.add("border-red-500"));
                    if (!firstErrorElement) firstErrorElement = section;
                }
            }

            /** 📌 6️⃣ 檢查必填的 `text` 輸入框 **/
            section.querySelectorAll(".required-text").forEach(textInput => {
                if (textInput.value.trim() === "") {
                    isValid = false;
                    sectionHasError = true;
                    textInput.classList.add("border-red-500");
                    if (!firstErrorElement) firstErrorElement = section;
                }
            });

            if (sectionHasError && sectionTitle) {
                errorSections.add(sectionTitle);
            }
        });

        // 顯示錯誤提示
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

    // 綁定表單提交按鈕
    document.getElementById("submitButton").addEventListener("click", function (event) {
        if (!validateForm()) {
            event.preventDefault();
        }
    });
});