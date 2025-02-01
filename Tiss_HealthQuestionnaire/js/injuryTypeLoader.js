// pastInjuryTypeLoader.js - 載入過去傷勢類型
function loadPastInjuryTypes(apiUrl, containerId) {
    fetch(apiUrl)
        .then(response => response.json())
        .then(data => {
            if (data.error) {
                console.error("錯誤:", data.error);
                return;
            }

            const injurySection = document.getElementById(containerId);
            if (!injurySection) {
                console.error("找不到 PastInjuryTypeSection 容器");
                return;
            }

            let content = "";

            // 依據分類顯示下拉選單
            for (const [category, items] of Object.entries(data)) {
                content += `<div>
                    <h3 class="font-bold mb-2 border-b border-gray-400">${category}</h3>
                    <select name="${category}" class="injury-type-select w-full border border-gray-300 p-2 rounded">
                        <option value="">請選擇</option>`;
                items.forEach(item => {
                    content += `<option value="${item}">${item}</option>`;
                });
                content += `</select></div>`;
            }

            injurySection.innerHTML = content; // 動態更新 HTML
        })
        .catch(error => console.error("載入過去傷勢類型時發生錯誤:", error));
}

// 確保 DOM 加載完成後執行
document.addEventListener("DOMContentLoaded", function () {
    loadPastInjuryTypes("/Questionnaire/PastInjuryType", "PastInjuryTypeSection");
});