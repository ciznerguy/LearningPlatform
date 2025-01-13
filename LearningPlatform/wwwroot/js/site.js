// wwwroot/js/site.js

console.log("site.js module loaded");

// פונקציה המפעילה את מצב המסך המלא
export function enterFullScreen() {
    let elem = document.documentElement;
    if (elem.requestFullscreen) {
        elem.requestFullscreen();
    } else if (elem.mozRequestFullScreen) {
        elem.mozRequestFullScreen();
    } else if (elem.webkitRequestFullscreen) {
        elem.webkitRequestFullscreen();
    } else if (elem.msRequestFullscreen) {
        elem.msRequestFullscreen();
    }
}

// פונקציה המציגה חלון מודאלי לבקשת סיסמה
function showPasswordModal() {
    // אם החלון כבר קיים, לא ליצור מחדש
    if (document.getElementById("exit-modal")) return;

    // יצירת שכבת overlay למילוי המסך
    const overlay = document.createElement("div");
    overlay.id = "exit-modal";
    overlay.style.position = "fixed";
    overlay.style.top = "0";
    overlay.style.left = "0";
    overlay.style.width = "100%";
    overlay.style.height = "100%";
    overlay.style.backgroundColor = "rgba(0,0,0,0.5)";
    overlay.style.display = "flex";
    overlay.style.alignItems = "center";
    overlay.style.justifyContent = "center";
    overlay.style.zIndex = "10000";

    // יצירת תיבת תוכן למודל
    const modalContent = document.createElement("div");
    modalContent.style.backgroundColor = "#fff";
    modalContent.style.padding = "20px";
    modalContent.style.borderRadius = "5px";
    modalContent.style.textAlign = "center";

    // טקסט הנחיה
    const promptText = document.createElement("p");
    promptText.innerText = "אנא הזן סיסמה ליציאה מהקיוסק מוד:";
    modalContent.appendChild(promptText);

    // שדה להזנת הסיסמה
    const inputField = document.createElement("input");
    inputField.type = "password";
    inputField.style.width = "80%";
    inputField.style.padding = "10px";
    inputField.style.marginBottom = "10px";
    modalContent.appendChild(inputField);

    // כפתור לאישור
    const submitButton = document.createElement("button");
    submitButton.innerText = "אישור";
    submitButton.style.padding = "10px 20px";
    submitButton.style.marginLeft = "10px";
    modalContent.appendChild(submitButton);

    // מאזין ללחיצה על הכפתור
    submitButton.addEventListener("click", function () {
        const pwd = inputField.value;
        if (pwd !== "1234") {
            alert("סיסמה שגויה! אתה חוזר לקיוסק מוד.");
            // קריאה מחדש ל-requestFullscreen נעשית במסגרת אירוע לחיצה – נחשבת ל-user gesture
            document.documentElement.requestFullscreen();
            inputField.value = "";
        } else {
            // אם הסיסמה נכונה – מסירים את המודל
            hidePasswordModal();
        }
    });

    overlay.appendChild(modalContent);
    document.body.appendChild(overlay);
}

// פונקציה להסרת המודל
function hidePasswordModal() {
    const modal = document.getElementById("exit-modal");
    if (modal) {
        modal.remove();
    }
}

// מאזין לשינוי במצב המסך מלא
document.addEventListener("fullscreenchange", () => {
    // אם אין אלמנט במצב מלא – להציג את החלון
    if (!document.fullscreenElement) {
        showPasswordModal();
    } else {
        // אם במסך מלא – להסיר את החלון (אם קיים)
        hidePasswordModal();
    }
});
