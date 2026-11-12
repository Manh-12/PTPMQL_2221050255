$(function () {
    const modalElement = document.getElementById("studentModal");
    if (!modalElement) {
        return;
    }

    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    const $modalBody = $("#studentModalBody");

    function showToast(message, isError) {
        const toastElement = document.getElementById("studentToast");
        $("#studentToastMessage").text(message);
        $(toastElement).toggleClass("text-bg-danger", Boolean(isError))
            .toggleClass("text-bg-success", !isError);
        bootstrap.Toast.getOrCreateInstance(toastElement).show();
    }

    function bindValidation() {
        const $form = $modalBody.find("form");
        if ($.validator && $.validator.unobtrusive) {
            $form.removeData("validator");
            $form.removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse($form);
        }
    }

    function renderStudentRow(student) {
        const $row = $("<tr>");
        [
            student.studentId,
            student.fullName,
            student.majorName,
            student.facultyName,
            student.trainingSystemName,
            student.address || ""
        ].forEach(function (value) {
            $("<td>").text(value).appendTo($row);
        });

        const $actions = $("<td>", { class: "text-center" });
        $("<a>", {
            class: "btn btn-sm btn-warning mb-1",
            href: "/Student/Edit/" + encodeURIComponent(student.studentId),
            text: "Sửa"
        }).appendTo($actions);
        $("<form>", {
            action: "/Student/Delete/" + encodeURIComponent(student.studentId),
            method: "post",
            class: "d-inline",
            "data-confirm-delete": "true"
        }).append(
            $("<input>", {
                type: "hidden",
                name: "__RequestVerificationToken",
                value: $modalBody.find('input[name="__RequestVerificationToken"]').val()
            }),
            $("<button>", {
                type: "submit",
                class: "btn btn-sm btn-danger mb-1",
                text: "Xóa"
            })
        ).appendTo($actions);
        $actions.appendTo($row);
        return $row;
    }

    $(document).on("show.bs.modal", "#studentModal", function () {
        const url = $("[data-student-create-url]").data("student-create-url");
        $modalBody.html(
            '<div class="d-flex justify-content-center py-4">' +
            '<div class="spinner-border text-primary" role="status">' +
            '<span class="visually-hidden">Đang tải...</span></div></div>'
        );

        $.get(url)
            .done(function (html) {
                $modalBody.html(html);
                bindValidation();
                $modalBody.find("[autofocus]").trigger("focus");
            })
            .fail(function () {
                $modalBody.html('<div class="alert alert-danger mb-0" role="alert">Không tải được biểu mẫu. Vui lòng thử lại.</div>');
            });
    });

    $(document).on("submit", "#createStudentForm", function (event) {
        event.preventDefault();

        const form = this;
        const $form = $(form);
        if ($form.data("submitting")) {
            return;
        }

        if ($.validator && !$form.valid()) {
            return;
        }

        $form.data("submitting", true);
        const $button = $form.find('button[type="submit"]');
        $button.prop("disabled", true);
        $button.find(".spinner-border").removeClass("d-none");
        $button.find(".submit-label").text("Đang lưu...");

        $.ajax({
            url: form.action,
            method: "POST",
            data: $form.serialize() + "&keyword=" +
                encodeURIComponent($('input[name="keyword"]').val() || ""),
            headers: { "X-Requested-With": "XMLHttpRequest" }
        }).done(function (result) {
            if (!result || !result.success || !result.student) {
                showToast("Máy chủ trả về kết quả không hợp lệ.", true);
                return;
            }

            const $body = $("#studentTableBody");
            if (result.matchesSearch) {
                if ($body.find("td[colspan]").length) {
                    $body.empty();
                }
                $body.prepend(renderStudentRow(result.student));

                const pageSize = Number($("#pageSize").val()) || 10;
                const rows = $body.find("tr").toArray().sort(function (left, right) {
                    return $(right).children().first().text()
                        .localeCompare($(left).children().first().text());
                });
                $body.empty().append(rows);
                while ($body.find("tr").length > pageSize) {
                    $body.find("tr").last().remove();
                }
            }
            $("#studentCountBadge, #studentTotalCount").text(result.totalCount);
            modal.hide();
            showToast(result.message, false);
        }).fail(function (xhr) {
            if ((xhr.status === 400 || xhr.status === 409) && xhr.responseText) {
                $modalBody.html(xhr.responseText);
                bindValidation();
                return;
            }

            showToast(xhr.status === 500
                ? "Đã xảy ra lỗi máy chủ khi lưu sinh viên."
                : "Không thể lưu sinh viên. Vui lòng thử lại.", true);
        }).always(function () {
            $form.data("submitting", false);
            $button.prop("disabled", false);
            $button.find(".spinner-border").addClass("d-none");
            $button.find(".submit-label").text("Lưu sinh viên");
        });
    });

    $(document).on("submit", "form[data-confirm-delete]", function (event) {
        if (!window.confirm("Bạn có chắc muốn xóa sinh viên này?")) {
            event.preventDefault();
        }
    });
});
