let steps = null;

function clearValidationErrors() {
    document.querySelectorAll(".border-red-500").forEach(el => el.classList.remove("border-red-500"));
}

function validateAllSteps() {
    if (!steps) steps = document.querySelectorAll(".question-step");

    clearValidationErrors();
    let firstErrorElement = null;
    const errorSections = [];

    steps.forEach((section, idx) => {
        const sectionTitle = section.dataset.sectionName || `第 ${idx + 1} 步`;
        let sectionHasError = false;
        const sectionName = section.dataset.sectionName;

        if (sectionName === "過去傷害(已復原)" || sectionName === "目前傷害狀況") {
            const isYes = section.querySelector(`input[name='${sectionName === "過去傷害(已復原)" ? "pastInjuryStatus" : "currentInjuryStatus"}'][value='yes']:checked`);
            if (isYes) {
                const partSelector = sectionName === "過去傷害(已復原)" ? ".pastInjury-injury-part-checkbox" : ".currentInjury-injury-part-checkbox";
                const typeSelector = sectionName === "過去傷害(已復原)" ? ".pastInjury-injury-type-checkbox" : ".currentInjury-injury-type-checkbox";
                const treatSelector = sectionName === "過去傷害(已復原)" ? "input[name='SelectedTreatmentMethods']" : "input[name='SelectedCurrentTreatmentMethods']";

                const part = section.querySelectorAll(partSelector);
                const type = section.querySelectorAll(typeSelector);
                const treat = section.querySelectorAll(treatSelector);

                const validPart = Array.from(part).some(cb => cb.checked);
                const validType = Array.from(type).some(cb => cb.checked);
                const validTreat = Array.from(treat).some(cb => cb.checked);

                if (!validPart || !validType || !validTreat) {
                    sectionHasError = true;
                    [...part, ...type, ...treat].forEach(cb => cb.classList.add("border-red-500"));
                    if (!firstErrorElement) firstErrorElement = section;
                }
            } else {
                return; // 若選「無」，略過驗證
            }
        }

        // 一般欄位驗證
        const radios = section.querySelectorAll("input.required-radio[type='radio']");
        const groupedNames = [...new Set(Array.from(radios).map(r => r.name))];
        groupedNames.forEach(name => {
            const group = section.querySelectorAll(`input[name='${name}']`);
            const hasChecked = Array.from(group).some(r => r.checked);
            if (!hasChecked) {
                sectionHasError = true;
                group.forEach(r => r.classList.add("border-red-500"));
                if (!firstErrorElement) firstErrorElement = section;
            }
        });

        section.querySelectorAll(".required-text").forEach(input => {
            if (input.offsetParent !== null && input.value.trim() === "") {
                sectionHasError = true;
                input.classList.add("border-red-500");
                if (!firstErrorElement) firstErrorElement = section;
            }
        });

        const checkboxes = section.querySelectorAll(".required-checkbox");
        if (checkboxes.length > 0) {
            const anyChecked = Array.from(checkboxes).some(cb => cb.checked);
            if (!anyChecked) {
                sectionHasError = true;
                checkboxes.forEach(cb => cb.classList.add("border-red-500"));
                if (!firstErrorElement) firstErrorElement = section;
            }
        }

        if (sectionHasError) {
            errorSections.push(sectionTitle);
        }
    });

    if (errorSections.length > 0) {
        Swal.fire({
            icon: "error",
            title: "請完成以下問卷項目",
            html: errorSections.map(name => `<b>${name}</b>`).join("<br>"),
            confirmButtonText: "確定"
        });

        if (firstErrorElement) {
            firstErrorElement.scrollIntoView({ behavior: "smooth", block: "center" });
        }

        return false;
    }

    return true;
}
