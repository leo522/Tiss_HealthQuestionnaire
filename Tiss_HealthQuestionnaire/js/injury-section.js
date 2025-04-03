// injury-section.js
const InjurySectionHandler = {
    init(sectionName) {
        const injuryStatusRadios = document.querySelectorAll(`input[name='${sectionName}Status']`);
        const detailSection = document.getElementById(`${sectionName}DetailSection`);
        const partCheckboxes = document.querySelectorAll(`.${sectionName}-injury-part-checkbox`);
        const typeSection = document.getElementById(`${sectionName}TypeSection`);
        const typeCheckboxes = document.querySelectorAll(`.${sectionName}-injury-type-checkbox`);
        const treatmentSection = document.getElementById(`${sectionName}TreatmentSection`);

        // ✅ 若找不到必要的元素，直接中止
        if (!detailSection || !typeSection || !treatmentSection) return;

        // 顯示或隱藏整個傷害區塊
        injuryStatusRadios.forEach(radio => {
            radio.addEventListener("change", () => {
                const show = radio.value === "yes" && radio.checked;
                detailSection.classList.toggle("hidden", !show);

                if (!show) {
                    typeSection.classList.add("hidden");
                    treatmentSection.classList.add("hidden");
                }
            });
        });

        // 勾選部位 => 顯示傷勢類型
        partCheckboxes.forEach(cb => {
            cb.addEventListener("change", () => {
                const anyChecked = Array.from(partCheckboxes).some(c => c.checked);
                if (!anyChecked) {
                    typeSection.classList.add("hidden");
                    treatmentSection.classList.add("hidden");
                } else {
                    typeSection.classList.remove("hidden");
                }
            });
        });

        // 勾選傷勢類型 => 顯示治療方式
        typeCheckboxes.forEach(cb => {
            cb.addEventListener("change", () => {
                const anyChecked = Array.from(typeCheckboxes).some(c => c.checked);
                if (!anyChecked) {
                    treatmentSection.classList.add("hidden");
                } else {
                    treatmentSection.classList.remove("hidden");
                }
            });
        });

        // 初始展開狀態處理
        const checkedRadio = Array.from(injuryStatusRadios).find(r => r.checked);
        if (checkedRadio && checkedRadio.value === "yes") {
            checkedRadio.dispatchEvent(new Event("change"));
        }
    }
};