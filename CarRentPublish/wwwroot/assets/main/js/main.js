///////////////////////
///////////////////////

//  Datatable

//////////////////////
//////////////////////
$(document).ready(function () {
  $(".full-table").DataTable({
    bFilter: true,
    //aaSorting: [],
    //pagingType: "simple",
    pageLength: 10,

    language: {
      sEmptyTable: "ليست هناك بيانات متاحة في الجدول",
      sLoadingRecords: "جارٍ التحميل...",
      sProcessing: "جارٍ التحميل...",
      sLengthMenu: "أظهر _MENU_ مدخلات",
      sZeroRecords: "لم يعثر على أية سجلات",
      sInfo: "إظهار _START_ إلى _END_ من أصل _TOTAL_ مدخل",
      sInfoEmpty: "يعرض 0 إلى 0 من أصل 0 سجل",
      sInfoFiltered: "(منتقاة من مجموع _MAX_ مُدخل)",
      sInfoPostFix: "",
      sSearch: "ابحث:",
      sUrl: "",
      oPaginate: {
        sFirst: "الأول",
        sPrevious: "السابق",
        sNext: "التالي",
        sLast: "الأخير",
      },
      oAria: {
        sSortAscending: ": تفعيل لترتيب العمود تصاعدياً",
        sSortDescending: ": تفعيل لترتيب العمود تنازلياً",
      },
      buttons: {
        copy: "نسخ",
        csv: "جدول",
        excel: "اكسيل",
        pdf: "ملف",
        print: "طباعة",
        colvis: "اختر بيانات",
      },
    },
    dom: "Bfrtip",
    buttons: ["excel", "pdf", "print", "colvis"],
    buttons: [
      {
        extend: "copy",
        exportOptions: {
          columns: ":visible",
        },
      },
      {
        extend: "csv",
        exportOptions: {
          columns: ":visible",
        },
      },
      {
        extend: "excel",
        exportOptions: {
          columns: ":visible",
        },
      },
      {
        extend: "pdf",
        exportOptions: {
          columns: ":visible",
        },
      },
      {
        extend: "print",
        exportOptions: {
          columns: ":visible",
        },
        customize: function (win) {
          //$(win.document.body).css('direction', 'rtl');
          //$(win.document.body).css('padding', '20px');
          if (document.getElementById("patient_search1")) {
            // some code here
            $(win.document.body).prepend(
              '<p>Patient Name:<span id="NName"> ' +
                document.getElementById("patient_search1_txt_name").value +
                " </span></p>"
            );
          }
        },
      },
      "colvis",
    ],
  });
});

///////////////////////
///////////////////////

//  Form group

//////////////////////
//////////////////////
$(document).ready(function () {
  $(".select2").select2({
    minimumResultsForSearch: 2,
    dropdownCssClass: "custome-select2",
  });
  $("#singleupload").uploadFile({
    url: "upload.php",
    multiple: false,
    dragDrop: false,
    maxFileCount: 1,
    fileName: "myfile",
  });
});
