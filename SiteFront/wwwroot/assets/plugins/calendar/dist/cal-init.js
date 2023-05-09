  
!function($) {
    "use strict";

    var CalendarApp = function() {
        this.$body = $("body")
        this.$calendar = $('#calendar'),
        this.$event = ('#calendar-events div.calendar-events'),
        this.$categoryForm = $('#add-new-event form'),
        this.$extEvents = $('#calendar-events'),
        this.$modal = $('#my-event'),
        this.$saveCategoryBtn = $('.save-category'),
        this.$calendarObj = null
    };

    /* on drop */
    CalendarApp.prototype.onDrop = function (eventObj, date, resourceId) { 
        var $this = this;
            // retrieve the dropped element's stored Event Object
            var originalEventObject = eventObj.data('eventObject');
            var $categoryClass = eventObj.attr('data-class');
            // we need to copy it, so that multiple events don't have a reference to the same object
            var copiedEventObject = $.extend({}, originalEventObject);
            // assign it the date that was reported
            copiedEventObject.start = date;
            copiedEventObject.resource = resourceId;
            if ($categoryClass)
                copiedEventObject['className'] = [$categoryClass];
            // render the event on the calendar
            $this.$calendar.fullCalendar('renderEvent', copiedEventObject, true);
            // is the "remove after drop" checkbox checked?
            if ($('#drop-remove').is(':checked')) {
                // if so, remove the element from the "Draggable Events" list
                eventObj.remove();
            }
    },
    /* on click on event */
    CalendarApp.prototype.onEventClick =  function (calEvent, jsEvent, view, date, resourceId) {
        var $this = this;
            var form = $("<form></form>");
            form.append("<label>Change Patient Name</label>");
            form.append("<div class='input-group'><input class='form-control' type=text value='" + calEvent.title + "' /><span class='input-group-btn'><button type='submit' class='btn btn-success'>Save</button></span></div>");
            $this.$modal.modal({
                backdrop: 'static'
            });
            $this.$modal.find('.delete-event').show().end().find('.save-event').hide().end().find('.modal-body').empty().prepend(form).end().find('.delete-event').unbind('click').click(function () {
                $this.$calendarObj.fullCalendar('removeEvents', function (ev) {
                    return (ev._id == calEvent._id);
                });
                $this.$modal.modal('hide');
            });
            $this.$modal.find('form').on('submit', function () {
                calEvent.title = form.find("input[type=text]").val();
                $this.$calendarObj.fullCalendar('updateEvent', calEvent);
                $this.$modal.modal('hide');
                return false;
            });
    },
    CalendarApp.prototype.enableDrag = function() {
        //init events
        $(this.$event).each(function () {
            // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
            // it doesn't need to have a start or end
            var eventObject = {
                title: $.trim($(this).text()) // use the element's text as the event title
            };
            // store the Event Object in the DOM element so we can get to it later
            $(this).data('eventObject', eventObject);
            // store data so the calendar knows to render an event upon drop
            $(this).data('event', {
                title: $.trim($(this).text()), // use the element's text as the event title
                stick: true, // maintain when user navigates (see docs on the renderEvent method)
                color: $(this).data("color")
            });
            // make the event draggable using jQuery UI
            $(this).draggable({
                zIndex: 999,
                revert: true,      // will cause the event to go back to its
                revertDuration: 0, //  original position after the drag
            });
        });
    }
    /* on select */
    CalendarApp.prototype.onSelect = function (start, end, jsEvent, view, resourceId) {
        var $this = this;
            $this.$modal.modal({
                backdrop: 'static'
            });
            var form = $("<form></form>");
            form.append("<div class='row'></div>");
            form.find(".row")
                .append("<div class='col-md-6'><div class='form-group'><label class='control-label'>Event Name</label><input class='form-control' placeholder='Insert Event Name' type='text' name='title'/></div></div>")
                .append("<div class='col-md-6'><div class='form-group'><label class='control-label'>Category</label><select class='form-control' name='category'></select></div></div>")
                .find("select[name='category']")
                .append("<option  class='#7996ff' style='background-color: #7996ff'>Examination</option>")
                .append("<option  class='#13dafe' style='background-color: #13dafe'>Consultation</option>")
                .append("<option  class='#dd636e' style='background-color: #dd636e'>Urgent</option>")
                .append("<option  class='#5ee39e' style='background-color: #5ee39e'>Vaccination</option>");
            $this.$modal.find('.delete-event').hide().end().find('.save-event').show().end().find('.modal-body').empty().prepend(form).end().find('.save-event').unbind('click').click(function () {
                form.submit();
            });
            $this.$modal.find('form').on('submit', function () {
                var title = form.find("input[name='title']").val();
                var beginning = form.find("input[name='beginning']").val();
                var ending = form.find("input[name='ending']").val();
                var categoryClass = form.find("select[name='category'] option:checked").val();
                if (title !== null && title.length != 0) {
                    $this.$calendarObj.fullCalendar('renderEvent', {
                        title: title,
                        start: start,
                        end: end,
                        resource: resource.id
                    }, true);  
                    $this.$modal.modal('hide');
                }
                else{
                    alert('You have to give a title to your event');
                }
                return false;
            });
            $this.$calendarObj.fullCalendar('unselect');
    },
    /* Initializing */
    CalendarApp.prototype.init = function() {
        this.enableDrag();
        /*  Initialize the calendar  */
        var date = new Date();
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();
        var form = '';
        var today = new Date($.now());

        var defaultEvents =  [{
                    id: '1',
                    title: 'Released Ample Admin!',
                    //start: new Date($.now() + 506800000),
                    start: today,
                    end: today,
                    resourceId: 'Q1',
                    className: 'bg-info'
                }, {
                    id: '2',
                    title: 'This is today check date',
                    start: today,
                    end: today,
                    resourceId: 'Q2',
                    className: 'bg-danger'
                }, {
                    id: '3',
                    title: 'This is your birthday',
                    //start: new Date($.now() + 848000000),
                    start: today,
                    end: today,
                    resourceId: 'Q3',
                    className: 'bg-info'
                },{
                    id: '4',
                    title: 'your meeting with john',
                    //start: new Date($.now() - 1099000000),
                    start: today,
                    end: today,
                    resourceId: 'Q4',
                    className: 'bg-warning'
                },{
                    id: '5',
                    title: 'your meeting with john',
                    //start: new Date($.now() - 1199000000),
                    start: today,
                    end: today,
                    resourceId: 'Q2',
                    className: 'bg-purple'
                },{
                    id: '6',
                    title: 'your meeting with john',
                    //start: new Date($.now() - 399000000),
                    start: today,
                    end: today,
                    resourceId: 'Q1',
                    className: 'bg-info'
                },  
                  {
                    id: '7',
                    title: 'Hanns birthday',
                    //start: new Date($.now() + 868000000),
                    start: today,
                    end: today,
                    resourceId: 'Q4',
                    className: 'bg-danger'
                },{
                    id: '8',
                    title: 'Like it?',
                    //start: new Date($.now() + 348000000),
                    start: today,
                    end: today,
                    resourceId: 'Q3',
                    className: 'bg-success'
                }
            ];

        var $this = this;
        $this.$calendarObj = $this.$calendar.fullCalendar({
            defaultView: 'agendaDay',
            timezone: 'local',
            slotDuration: '00:30:00', /* If we want to split day time each 15minutes */
            minTime: '00:00:00',
            maxTime: '24:00:00',
            firstDay: 6,
            handleWindowResize: true,
            allDaySlot: false,
            editable: true,
            droppable: true,
            eventLimit: true,
            selectable: true,
            defaultTimedEventDuration: '01:00:00',
            forceEventDuration: true,
            groupByDateAndResource:true,
            events: defaultEvents,
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'agendaDay,agendaTwoDay,agendaFourDay,agendaWeek,month'
            },
            views: {
                agendaTwoDay: {
                    type: 'agenda',
                    duration: { days: 2 },
                    groupByResource: true
                },
                agendaFourDay: {
                    type: 'agenda',
                    duration: { days: 4 },
                    groupByResource: true
                },
                agendaWeek: {
                    groupByResource: true
                },
            },
            resourceGroupField:true,
            resourceColumns: [
                {
                    group: true,
                    labelText: 'Doctor',
                    field: 'Doctor'
                },
                {
                    labelText: 'Hour',
                    field: 'title'
                }
            ],
            resources: [
                { id: 'Q1', Doctor: 'Doctor1', title: '00:15' },
                { id: 'Q2', Doctor: 'Doctor1', title: '15:30' },
                { id: 'Q3', Doctor: 'Doctor1', title: '30:45' },
                { id: 'Q4', Doctor: 'Doctor1', title: '45:60' },
            ],
            /*
            eventDrop: function(event) { // called when an event (already on the calendar) is moved
                console.log('eventDrop', event);

            eventClick: function(calEvent, jsEvent, view, date, resourceId) {
                $this.onEventClick(calEvent, jsEvent, view, date, resource);
                console.log(
                    'eventClick',
                    date.format(),
                    resource ? resource.id : '(no resource)'
                );
            },
//select: function (start, end) { $this.onSelect(start, end); },

resourceRender: function(resource, element, view) {
    // this is triggered when the resource is rendered, just like eventRender
},
            },*/

            dayClick: function(date, allDay, jsEvent, view) {
                var DaTe = date.format();
                if (allDay) {
                    // Clicked on the entire day
                    $('#calendar').fullCalendar('changeView', 'agendaDay'/* or 'basicDay' */);
                    $('#calendar').fullCalendar('gotoDate', DaTe);
                }
            },

            select: function(start, end, jsEvent, view, resource) {
                //var title = prompt('event title:');
                $this.onSelect(start, end, jsEvent, view, resource);
                console.log(
                    resource ? resource.id : '(no resource)'
                );
            },


eventClick: function ( event, jsEvent, view )  {
    alert('event title "'+event.title+'" in ' +event.resourceId);
},
eventDrop: function( event, dayDelta, minuteDelta, allDay, revertFunc, jsEvent, ui, view ) {
    alert('event was moved, start time: '+event.start.format()+' in '+ event.resourceId);
},
eventResize: function( event, dayDelta, minuteDelta, allDay, revertFunc, jsEvent, ui, view ) { 
    alert('event was resized, new end time: '+event.end.format());
},
drop: function(date) {
    $this.onDrop($(this), date);
},
            
            eventReceive: function(event) { // called when a proper external event is dropped
                console.log('eventReceive', event);
            },
        });

    $('#datepicker').datepicker({
        inline: true,
        showOn: "button",
        buttonText: "Change Date",
        onSelect: function(dateText, inst) {
            var d = new Date(dateText);
            $('#calendar').fullCalendar('gotoDate', d);
        }
    });
        //on new event
        this.$saveCategoryBtn.on('click', function(){
            var categoryName = $this.$categoryForm.find("input[id='category-name']").val();
            var categoryColor = $this.$categoryForm.find("input[id='category-color']").val();
            if (categoryName !== null && categoryName.length != 0) {
                $this.$extEvents.append('<div class="calendar-events d-inline-block" data-color="' + categoryColor + '" style="background-color:' + categoryColor + ';position: relative;">' + categoryName + '</div> ')
                $this.enableDrag();
            }
        });
    },
   //init CalendarApp
    $.CalendarApp = new CalendarApp, $.CalendarApp.Constructor = CalendarApp
    
}(window.jQuery),

//initializing CalendarApp
function($) {
    "use strict";
    $.CalendarApp.init()
}(window.jQuery);

/////////////////////////////////////////
/////////////////////////////////////////
$(function() {

  $('#daysTime').fullCalendar({
    defaultView: 'timeline',
    timezone: 'local',
    slotDuration: '00:30:00', /* If we want to split day time each 15minutes */
    minTime: '00:00:00',
    maxTime: '24:00:00',
    handleWindowResize: true,
    allDaySlot: false,
    editable: true,
    selectable: true,
    slotLabelFormat:"HH:mm a",
    resourceAreaWidth: 150,
    resourceLabelText: 'Week Days',
    header: {
        left: '',
        center: '',
        right: ''
    },
    resources: [
      { id: 'Sat', title: 'Saturday' },
      { id: 'Sun', title: 'Sunday' },
      { id: 'Mon', title: 'Monday' },
      { id: 'Tue', title: 'Tuesday' },
      { id: 'Wed', title: 'Wednesday' },
      { id: 'Thu', title: 'Thursday' },
      { id: 'Fri', title: 'Friday' },
    ],
    dayClick: function(date, jsEvent, view, resource) {
      alert('clicked ' + date.format() + ' on resource ' + resource.id);
    },
    select: function(startDate, endDate, jsEvent, view, resource) {
      alert('selected ' + startDate.format("HH:mm a") + ' to ' + endDate.format("HH:mm a") + ' Day ' + resource.id);
    }
  });
});