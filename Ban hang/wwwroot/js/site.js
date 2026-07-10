(function () {
  "use strict";

  /* ---------- Header scroll state ---------- */
  var header = document.getElementById("siteHeader");
  var toTop = document.getElementById("toTop");
  window.addEventListener(
    "scroll",
    function () {
      var y = window.scrollY || document.documentElement.scrollTop;
      header.classList.toggle("scrolled", y > 12);
      toTop.classList.toggle("show", y > 600);
    },
    { passive: true },
  );

  toTop.addEventListener("click", function () {
    window.scrollTo({ top: 0, behavior: "smooth" });
  });

  /* ---------- Mobile nav ---------- */
  var navToggle = document.getElementById("navToggle");
  var mobilePanel = document.getElementById("mobilePanel");
  function closeMobile() {
    mobilePanel.classList.remove("open");
    navToggle.setAttribute("aria-expanded", "false");
  }
  navToggle.addEventListener("click", function () {
    var open = mobilePanel.classList.toggle("open");
    navToggle.setAttribute("aria-expanded", open ? "true" : "false");
  });
  mobilePanel.querySelectorAll("a").forEach(function (a) {
    a.addEventListener("click", closeMobile);
  });

  /* ---------- Scroll reveal ---------- */
  var revealEls = document.querySelectorAll(".reveal");
  if ("IntersectionObserver" in window) {
    var io = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (entry.isIntersecting) {
            entry.target.classList.add("in");
            io.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.12, rootMargin: "0px 0px -40px 0px" },
    );
    revealEls.forEach(function (el) {
      io.observe(el);
    });
  } else {
    revealEls.forEach(function (el) {
      el.classList.add("in");
    });
  }

  /* ---------- Stat counters ---------- */
  var statEls = document.querySelectorAll("[data-count]");
  var statsDone = false;
  function animateStats() {
    if (statsDone) return;
    statsDone = true;
    statEls.forEach(function (el) {
      var target = parseInt(el.getAttribute("data-count"), 10);
      var decimalDiv = el.getAttribute("data-decimal");
      var suffix = el.getAttribute("data-suffix") || "";
      var duration = 1400;
      var start = null;
      function step(ts) {
        if (!start) start = ts;
        var progress = Math.min((ts - start) / duration, 1);
        var eased = 1 - Math.pow(1 - progress, 3);
        var value = target * eased;
        var display = decimalDiv
          ? (value / parseInt(decimalDiv, 10)).toFixed(1)
          : Math.floor(value).toLocaleString("vi-VN");
        el.textContent = display + suffix;
        if (progress < 1) requestAnimationFrame(step);
      }
      requestAnimationFrame(step);
    });
  }
  var statsBand = document.querySelector(".stats-band");
  if (statsBand && "IntersectionObserver" in window) {
    var statIo = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (e) {
          if (e.isIntersecting) animateStats();
        });
      },
      { threshold: 0.4 },
    );
    statIo.observe(statsBand);
  } else if (statsBand) {
    animateStats();
  }

  /* ---------- Gallery filter ---------- */
  var galFilters = document.getElementById("galFilters");
  var galItems = document.querySelectorAll(".gal-item");
  if (galFilters) {
    galFilters.addEventListener("click", function (e) {
      var btn = e.target.closest("button");
      if (!btn) return;
      galFilters.querySelectorAll("button").forEach(function (b) {
        b.classList.remove("active");
      });
      btn.classList.add("active");
      var filter = btn.getAttribute("data-filter");
      galItems.forEach(function (item) {
        var show =
          filter === "all" || item.getAttribute("data-cat") === filter;
        item.style.display = show ? "" : "none";
      });
    });
  }

  /* ---------- Lightbox ---------- */
  var lightbox = document.getElementById("lightbox");
  var lbImage = document.getElementById("lbImage");
  var lbCaption = document.getElementById("lbCaption");
  var galArray = Array.prototype.slice.call(galItems);
  var currentIndex = 0;

  function openLightbox(index) {
    currentIndex = index;
    var item = galArray[currentIndex];
    var img = item.querySelector("img");
    lbImage.src = img.src;
    lbImage.alt = img.alt;
    lbCaption.textContent = item.getAttribute("data-caption") || "";
    lightbox.classList.add("open");
    document.body.style.overflow = "hidden";
  }
  function closeLightbox() {
    lightbox.classList.remove("open");
    document.body.style.overflow = "";
  }
  function showRelative(delta) {
    currentIndex =
      (currentIndex + delta + galArray.length) % galArray.length;
    var item = galArray[currentIndex];
    var img = item.querySelector("img");
    lbImage.src = img.src;
    lbImage.alt = img.alt;
    lbCaption.textContent = item.getAttribute("data-caption") || "";
  }

  galArray.forEach(function (item, idx) {
    item.addEventListener("click", function () {
      openLightbox(idx);
    });
  });
  if (lightbox) {
    document.getElementById("lbClose").addEventListener("click", closeLightbox);
    document.getElementById("lbPrev").addEventListener("click", function () {
      showRelative(-1);
    });
    document.getElementById("lbNext").addEventListener("click", function () {
      showRelative(1);
    });
    lightbox.addEventListener("click", function (e) {
      if (e.target === lightbox) closeLightbox();
    });
    document.addEventListener("keydown", function (e) {
      if (!lightbox.classList.contains("open")) return;
      if (e.key === "Escape") closeLightbox();
      if (e.key === "ArrowLeft") showRelative(-1);
      if (e.key === "ArrowRight") showRelative(1);
    });
  }

  /* ---------- Pricing toggle ---------- */
  var petToggle = document.getElementById("petToggle");
  var priceAmounts = document.querySelectorAll(".price-amount");
  if (petToggle) {
    petToggle.addEventListener("click", function (e) {
      var btn = e.target.closest("button");
      if (!btn) return;
      petToggle.querySelectorAll("button").forEach(function (b) {
        b.classList.remove("active");
      });
      btn.classList.add("active");
      var pet = btn.getAttribute("data-pet");
      priceAmounts.forEach(function (el) {
        var price = el.getAttribute("data-" + pet);
        var span = el.querySelector("span");
        el.firstChild.textContent = price + " ";
        if (span) el.appendChild(span);
      });
    });
  }

  /* ---------- Testimonial carousel ---------- */
  var testiSlides = document.getElementById("testiSlides");
  if (testiSlides) {
    var slides = testiSlides.children;
    var testiDots = document.getElementById("testiDots");
    var testiIndex = 0;

    for (var i = 0; i < slides.length; i++) {
      var dot = document.createElement("button");
      if (i === 0) dot.classList.add("active");
      dot.setAttribute("aria-label", "Đánh giá số " + (i + 1));
      (function (idx) {
        dot.addEventListener("click", function () {
          goToTesti(idx);
        });
      })(i);
      testiDots.appendChild(dot);
    }

    function goToTesti(idx) {
      testiIndex = (idx + slides.length) % slides.length;
      testiSlides.style.transform = "translateX(-" + testiIndex * 100 + "%)";
      testiDots.querySelectorAll("button").forEach(function (d, i2) {
        d.classList.toggle("active", i2 === testiIndex);
      });
    }
    document.getElementById("testiPrev").addEventListener("click", function () {
      goToTesti(testiIndex - 1);
    });
    document.getElementById("testiNext").addEventListener("click", function () {
      goToTesti(testiIndex + 1);
    });

    var testiTimer = setInterval(function () {
      goToTesti(testiIndex + 1);
    }, 6000);
    var testiWrap = document.querySelector(".testi-wrap");
    testiWrap.addEventListener("mouseenter", function () {
      clearInterval(testiTimer);
    });
    testiWrap.addEventListener("mouseleave", function () {
      testiTimer = setInterval(function () {
        goToTesti(testiIndex + 1);
      }, 6000);
    });
  }

  /* ---------- FAQ accordion ---------- */
  var faqList = document.getElementById("faqList");
  if (faqList) {
    faqList.addEventListener("click", function (e) {
      var q = e.target.closest(".faq-q");
      if (!q) return;
      var item = q.closest(".faq-item");
      var wasOpen = item.classList.contains("open");
      faqList.querySelectorAll(".faq-item").forEach(function (it) {
        it.classList.remove("open");
      });
      if (!wasOpen) item.classList.add("open");
    });
  }

  /* ---------- Toast helper ----------
     Still used as a light client-side nicety (e.g. you could call
     showToast() from a fetch() success handler if you later turn the
     newsletter form into an AJAX call instead of a normal POST). */
  function showToast(message) {
    var region = document.getElementById("toastRegion");
    var toast = document.createElement("div");
    toast.className = "toast";
    toast.innerHTML =
      '<svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.4" stroke-linecap="round" stroke-linejoin="round"><path d="M20 6 9 17l-5-5"/></svg><span></span>';
    toast.querySelector("span").textContent = message;
    region.appendChild(toast);
    requestAnimationFrame(function () {
      toast.classList.add("show");
    });
    setTimeout(function () {
      toast.classList.remove("show");
      setTimeout(function () {
        toast.remove();
      }, 300);
    }, 3200);
  }
  window.showToast = showToast;

  /* ---------- Booking form date default ---------- */
  var bookDateInput = document.getElementById("Booking_BookDate");
  if (bookDateInput) {
    var todayStr = new Date().toISOString().split("T")[0];
    bookDateInput.setAttribute("min", todayStr);
  }

  // NOTE: The custom client-side validation + preventDefault() that the
  // static HTML version had for #bookingForm and #newsletterForm is gone.
  // Both forms now POST for real, to HomeController.BookingSubmit /
  // HomeController.NewsletterSubmit. Validation comes from the
  // [Required]/[RegularExpression] attributes on BookingViewModel /
  // NewsletterViewModel — enforced server-side via ModelState, and
  // client-side "for free" via jQuery Unobtrusive Validation (loaded
  // in _Layout.cshtml) reading the asp-validation-for spans.
})();
// ============ ACTIVE NAVBAR THEO SECTION ĐANG XEM ============
(function () {
    const navLinks = document.querySelectorAll('.nav-links a[href*="#"]');
    const sections = document.querySelectorAll('section[id]');

    function setActiveLink() {
        let currentSection = "";

        sections.forEach(section => {
            const sectionTop = section.offsetTop - 120;
            const sectionHeight = section.offsetHeight;
            if (window.scrollY >= sectionTop && window.scrollY < sectionTop + sectionHeight) {
                currentSection = section.getAttribute('id');
            }
        });

        navLinks.forEach(link => {
            link.classList.remove('active');
            const href = link.getAttribute('href') || "";
            if (href.includes('#' + currentSection) && currentSection !== "") {
                link.classList.add('active');
            }
        });
    }

    window.addEventListener('scroll', setActiveLink);
    setActiveLink();
})();
