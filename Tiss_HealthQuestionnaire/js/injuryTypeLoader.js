/*過去傷勢*/
document.addEventListener("DOMContentLoaded", function () {
    const pastNoInjury = document.getElementById("pastNoInjury");
    const pastHasInjury = document.getElementById("pastHasInjury");
    const injuryPartDetails = document.getElementById("injuryPartDetails");
    const pastInjuryTypeSection = document.getElementById("allInjuryTypesSection");
    const pastInjuryTypeContainer = pastInjuryTypeSection.querySelector(".grid");
    const treatmentMethodSection = document.getElementById("treatmentMethodSection");
    const treatmentMethodContainer = treatmentMethodSection.querySelector(".grid");

    if (!injuryPartDetails || !pastInjuryTypeSection || !pastInjuryTypeContainer || !treatmentMethodSection || !treatmentMethodContainer) {
        console.error("❌ 找不到必要的 HTML 元素，請確認 HTML 結構是否正確");
        return;
    }

    // **控制傷害部位的顯示/隱藏**
    function togglePastInjurySections() {
        if (pastHasInjury.checked) {
            injuryPartDetails.classList.remove("hidden");
            pastInjuryTypeSection.classList.remove("hidden");
            treatmentMethodSection.classList.add("hidden"); // **預設隱藏治療方式**
            loadAllPastInjuryTypes();
        } else {
            injuryPartDetails.classList.add("hidden");
            pastInjuryTypeSection.classList.add("hidden");
            treatmentMethodSection.classList.add("hidden");
            pastInjuryTypeContainer.innerHTML = "";
            treatmentMethodContainer.innerHTML = "";
        }
    }

    // **載入所有傷勢類型**
    function loadAllPastInjuryTypes() {
        fetch(`/Questionnaire/PastInjuryType?all=true`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP 錯誤！狀態碼：${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                console.log("✅ API 回傳所有傷勢類型:", data);
                pastInjuryTypeContainer.innerHTML = "";

                if (!data || Object.keys(data).length === 0) {
                    pastInjuryTypeSection.classList.add("hidden");
                    return;
                }

                let content = "";
                for (const [category, injuries] of Object.entries(data)) {
                    content += `
                    <div class="p-2 border rounded-md shadow">
                        <h3 class="font-bold mb-2 border-b border-gray-400">${category}</h3>
                        <select class="w-full border border-gray-300 p-2 rounded past-injury-type">
                            <option value="">請選擇</option>`;
                    injuries.forEach(injury => {
                        content += `<option value="${injury}">${injury}</option>`;
                    });
                    content += `</select>
                    </div>`;
                }

                pastInjuryTypeContainer.innerHTML = content;
                pastInjuryTypeSection.classList.remove("hidden");

                // **監聽傷勢類型選擇**
                document.querySelectorAll(".past-injury-type").forEach(select => {
                    select.addEventListener("change", loadTreatmentMethods);
                });
            })
            .catch(error => {
                console.error("❌ 載入過去傷勢類型時發生錯誤:", error);
            });
    }

    // **根據選擇的傷勢類型載入治療方式**
    function loadTreatmentMethods() {
        fetch(`/Questionnaire/PastTreatmentMethod`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP 錯誤！狀態碼：${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                console.log("✅ API 回傳治療方式:", data);
                treatmentMethodContainer.innerHTML = "";

                if (!data || data.length === 0) {
                    treatmentMethodSection.classList.add("hidden");
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

                treatmentMethodContainer.innerHTML = content;
                treatmentMethodSection.classList.remove("hidden");
            })
            .catch(error => {
                console.error("❌ 載入治療方式時發生錯誤:", error);
            });
    }

    pastNoInjury.addEventListener("change", togglePastInjurySections);
    pastHasInjury.addEventListener("change", togglePastInjurySections);

    injuryPartDetails.classList.add("hidden");
    pastInjuryTypeSection.classList.add("hidden");
    treatmentMethodSection.classList.add("hidden");
});