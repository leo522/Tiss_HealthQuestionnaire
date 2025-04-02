document.addEventListener("DOMContentLoaded", function () {
    const currentNoInjury = document.getElementById("currentNoInjury");
    const currentHasInjury = document.getElementById("currentHasInjury");
    const currentInjuryDetails = document.getElementById("currentInjuryDetails");
    const currentInjuryTypeSection = document.getElementById("currentInjuryTypesSection");
    const currentInjuryTypeContainer = currentInjuryTypeSection.querySelector(".grid");
    const currentTreatmentMethodSection = document.getElementById("currentTreatmentMethodSection");
    const currentTreatmentMethodContainer = currentTreatmentMethodSection.querySelector(".grid");

    if (!currentInjuryDetails || !currentInjuryTypeSection || !currentInjuryTypeContainer || !currentTreatmentMethodSection || !currentTreatmentMethodContainer) {
        console.error("❌ 找不到必要的 HTML 元素，請確認 HTML 結構是否正確");
        return;
    }

    function toggleCurrentInjurySections() {
        if (currentHasInjury.checked) {
            currentInjuryDetails.classList.remove("hidden");
            currentInjuryTypeSection.classList.remove("hidden");
            currentTreatmentMethodSection.classList.add("hidden");
            loadAllCurrentInjuryTypes();
        } else {
            currentInjuryDetails.classList.add("hidden");
            currentInjuryTypeSection.classList.add("hidden");
            currentTreatmentMethodSection.classList.add("hidden");
            currentInjuryTypeContainer.innerHTML = "";
            currentTreatmentMethodContainer.innerHTML = "";
        }
    }

    function loadAllCurrentInjuryTypes() {
        fetch(`/Questionnaire/CurrentInjuryType?all=true`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP 錯誤！狀態碼：${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                console.log("✅ API 回傳所有傷勢類型:", data);
                currentInjuryTypeContainer.innerHTML = "";

                if (!data || Object.keys(data).length === 0) {
                    currentInjuryTypeSection.classList.add("hidden");
                    return;
                }

                let content = "";
                for (const [category, injuries] of Object.entries(data)) {
                    content += `
                    <div class="p-2 border rounded-md shadow">
                        <h3 class="font-bold mb-2 border-b border-gray-400">${category}</h3>
                        <select class="w-full border border-gray-300 p-2 rounded current-injury-type">
                            <option value="">請選擇</option>`;
                    injuries.forEach(injury => {
                        content += `<option value="${injury}">${injury}</option>`;
                    });
                    content += `</select>
                    </div>`;
                }

                currentInjuryTypeContainer.innerHTML = content;
                currentInjuryTypeSection.classList.remove("hidden");

                document.querySelectorAll(".current-injury-type").forEach(select => {
                    select.addEventListener("change", loadCurrentTreatmentMethods);
                });
            })
            .catch(error => {
                console.error("❌ 載入目前傷勢類型時發生錯誤:", error);
            });
    }

    function loadCurrentTreatmentMethods() {
        fetch(`/Questionnaire/CurrentTreatmentMethod`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP 錯誤！狀態碼：${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                console.log("✅ API 回傳治療方式:", data);
                currentTreatmentMethodContainer.innerHTML = "";

                if (!data || data.length === 0) {
                    currentTreatmentMethodSection.classList.add("hidden");
                    return;
                }

                let content = `
                <div class="p-2 border rounded-md shadow">
                    <h3 class="font-bold mb-2 border-b border-gray-400">治療方式</h3>`;
                data.forEach(method => {
                    content += `
                    <label class="block">
                        <input type="checkbox" name="treatmentMethod" value="${method.Method}" class="form-checkbox h-5 w-5 text-blue-600 mr-2">
                        ${method.Method}
                    </label>`;
                });
                content += `</div>`;

                currentTreatmentMethodContainer.innerHTML = content;
                currentTreatmentMethodSection.classList.remove("hidden");
            })
            .catch(error => {
                console.error("❌ 載入治療方式時發生錯誤:", error);
            });
    }

    currentNoInjury.addEventListener("change", toggleCurrentInjurySections);
    currentHasInjury.addEventListener("change", toggleCurrentInjurySections);

    currentInjuryDetails.classList.add("hidden");
    currentInjuryTypeSection.classList.add("hidden");
    currentTreatmentMethodSection.classList.add("hidden");
});
