(() => {
  const avatarBtn = document.querySelector(".avatar-btn");
  const dropdownMenu = avatarBtn?.closest(".dropdown")?.querySelector(".dropdown-menu");

  if (!avatarBtn || !dropdownMenu) return;

  const closeMenu = () => {
    dropdownMenu.classList.remove("show");
    avatarBtn.setAttribute("aria-expanded", "false");
  };

  const toggleMenu = () => {
    const isOpen = dropdownMenu.classList.contains("show");
    if (isOpen) {
      closeMenu();
      return;
    }
    dropdownMenu.classList.add("show");
    avatarBtn.setAttribute("aria-expanded", "true");
  };

  avatarBtn.addEventListener("click", (event) => {
    event.stopPropagation();
    toggleMenu();
  });

  document.addEventListener("click", () => {
    closeMenu();
  });

  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") {
      closeMenu();
    }
  });
})();

(() => {
  const noteInput = document.querySelector("#StatusNote");
  const counter = document.querySelector("#status-note-count");

  if (!noteInput || !counter) return;

  const max = 50;
  const updateCounter = () => {
    const length = noteInput.value.length;
    const remaining = Math.max(0, max - length);
    counter.textContent = `${length}/${max}`;
    counter.classList.toggle("is-danger", remaining <= 5);
  };

  noteInput.addEventListener("input", updateCounter);
  updateCounter();
})();

(() => {
  const truncates = document.querySelectorAll(".truncate");
  if (!truncates.length) return;
  truncates.forEach((el) => {
    const text = el.textContent?.trim();
    if (!text) return;
    if (el.scrollWidth > el.clientWidth) {
      el.setAttribute("title", text);
    } else {
      el.removeAttribute("title");
    }
  });
})();

(() => {
  const logoutForms = document.querySelectorAll("form[data-confirm]");
  if (!logoutForms.length) return;
  logoutForms.forEach((form) => {
    form.addEventListener("submit", (event) => {
      const message = form.getAttribute("data-confirm") || "Are you sure?";
      if (!window.confirm(message)) {
        event.preventDefault();
      }
    });
  });
})();

(() => {
  const banner = document.querySelector("[data-inbox-poll]");
  if (!banner) return;
  const afterTicks = Number(banner.getAttribute("data-after") || "0");
  if (!afterTicks) return;

  const poll = async () => {
    try {
      const response = await fetch(`/Messages/HasNew?afterTicks=${afterTicks}`, {
        headers: { "Accept": "application/json" }
      });
      if (!response.ok) return;
      const data = await response.json();
      if (data?.hasNew) {
        banner.style.display = "block";
      }
    } catch {
      // ignore polling errors
    }
  };

  setInterval(poll, 15000);
})();

(() => {
  const modal = document.querySelector("#message-modal");
  if (!modal) return;
  const recipientIdInput = document.querySelector("#message-recipient-id");
  const recipientNameInput = document.querySelector("#message-recipient-name");
  const subjectInput = document.querySelector("#message-subject");
  const bodyInput = document.querySelector("#message-body");
  const modalForm = modal.querySelector("[data-modal-form]");

  const openModal = (id, name) => {
    recipientIdInput.value = id;
    recipientNameInput.value = name;
    subjectInput.value = "";
    bodyInput.value = "";
    modal.classList.add("is-open");
    modal.setAttribute("aria-hidden", "false");
    subjectInput.focus();
  };

  const closeModal = () => {
    modal.classList.remove("is-open");
    modal.setAttribute("aria-hidden", "true");
  };

  document.querySelectorAll(".message-btn").forEach((btn) => {
    btn.addEventListener("click", () => {
      openModal(btn.getAttribute("data-user-id"), btn.getAttribute("data-user-name"));
    });
  });

  modal.querySelectorAll("[data-modal-close]").forEach((el) => {
    el.addEventListener("click", closeModal);
  });

  if (modalForm) {
    modalForm.addEventListener("submit", async (event) => {
      event.preventDefault();
      const formData = new FormData(modalForm);
      const returnUrl = modalForm.getAttribute("data-return-url") || "/";
      try {
        const response = await fetch(modalForm.action, {
          method: "POST",
          body: formData,
          headers: { "X-Requested-With": "XMLHttpRequest" }
        });
        if (response.ok) {
          closeModal();
          window.location.href = returnUrl;
        }
      } catch {
        // swallow and allow normal form submit fallback if needed
        modalForm.submit();
      }
    });
  }
})();
