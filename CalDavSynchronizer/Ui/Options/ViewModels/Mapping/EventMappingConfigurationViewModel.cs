﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using CalDavSynchronizer.Contracts;
using CalDavSynchronizer.Utilities;
using Microsoft.Office.Interop.Outlook;
using System.Linq;

namespace CalDavSynchronizer.Ui.Options.ViewModels.Mapping
{
  public class EventMappingConfigurationViewModel : ViewModelBase, IOptionsViewModel
  {
    private OlCategoryShortcutKey _categoryShortcutKey;
    private bool _createEventsInUtc;
    private string _eventCategory;
    private OlCategoryColor _eventCategoryColor;
    private bool _invertEventCategoryFilter;
    private bool _mapAttendees;
    private bool _mapBody;
    private bool _mapClassConfidentialToSensitivityPrivate;
    private ReminderMapping _mapReminder;
    private bool _mapSensitivityPrivateToClassConfidential;
    private bool _scheduleAgentClient;
    private bool _sendNoAppointmentNotifications;
    private bool _useEventCategoryColorAndMapFromCalendarColor;

    public IList<Item<ReminderMapping>> AvailableReminderMappings { get; } = new List<Item<ReminderMapping>>
                                                                             {
                                                                                 new Item<ReminderMapping> (ReminderMapping.@true, "Yes"),
                                                                                 new Item<ReminderMapping> (ReminderMapping.@false, "No"),
                                                                                 new Item<ReminderMapping> (ReminderMapping.JustUpcoming, "Just upcoming reminders")
                                                                             };

    public IList<Item<OlCategoryShortcutKey>> AvailableShortcutKeys { get; } = new List<Item<OlCategoryShortcutKey>>
                                                                               {
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyNone, "None"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF2, "Ctrl+F2"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF3, "Ctrl+F3"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF4, "Ctrl+F4"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF5, "Ctrl+F5"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF6, "Ctrl+F6"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF7, "Ctrl+F7"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF8, "Ctrl+F8"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF9, "Ctrl+F9"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF10, "Ctrl+F10"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF11, "Ctrl+F11"),
                                                                                   new Item<OlCategoryShortcutKey> (OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF12, "Ctrl+F12")
                                                                               };

    public IList<Item<OlCategoryColor>> AvailableEventCategoryColors { get; } = 
      ColorHelper.CategoryColors.Select (kv => new Item<OlCategoryColor> (kv.Key, kv.Key.ToString().Substring (15))).ToList();

    public OlCategoryShortcutKey CategoryShortcutKey
    {
      get { return _categoryShortcutKey; }
      set
      {
        _categoryShortcutKey = value;
        OnPropertyChanged();
      }
    }

    public bool CreateEventsInUtc
    {
      get { return _createEventsInUtc; }
      set
      {
        _createEventsInUtc = value;
        OnPropertyChanged();
      }
    }

    public string EventCategory
    {
      get { return _eventCategory; }
      set
      {
        _eventCategory = value;
        OnPropertyChanged();
        // ReSharper disable once ExplicitCallerInfoArgument
        OnPropertyChanged(nameof(UseEventCategoryAsFilter));
        // ReSharper disable once ExplicitCallerInfoArgument
        OnPropertyChanged (nameof(UseEventCategoryAsFilterAndMapColor));
      }
    }

    public bool UseEventCategoryAsFilter => !String.IsNullOrEmpty (_eventCategory);
    public bool UseEventCategoryAsFilterAndMapColor => !String.IsNullOrEmpty (_eventCategory) && _useEventCategoryColorAndMapFromCalendarColor;


    public OlCategoryColor EventCategoryColor
    {
      get { return _eventCategoryColor; }
      set
      {
        _eventCategoryColor = value;
        OnPropertyChanged();
      }
    }

    public bool InvertEventCategoryFilter
    {
      get { return _invertEventCategoryFilter; }
      set
      {
        _invertEventCategoryFilter = value;
        OnPropertyChanged();
      }
    }

    public bool MapAttendees
    {
      get { return _mapAttendees; }
      set
      {
        _mapAttendees = value;
        OnPropertyChanged();
      }
    }

    public bool MapBody
    {
      get { return _mapBody; }
      set
      {
        _mapBody = value;
        OnPropertyChanged();
      }
    }

    public bool MapClassConfidentialToSensitivityPrivate
    {
      get { return _mapClassConfidentialToSensitivityPrivate; }
      set
      {
        _mapClassConfidentialToSensitivityPrivate = value;
        OnPropertyChanged();
      }
    }

    public ReminderMapping MapReminder
    {
      get { return _mapReminder; }
      set
      {
        _mapReminder = value;
        OnPropertyChanged();
      }
    }

    public bool MapSensitivityPrivateToClassConfidential
    {
      get { return _mapSensitivityPrivateToClassConfidential; }
      set
      {
        _mapSensitivityPrivateToClassConfidential = value;
        OnPropertyChanged();
      }
    }

    public bool ScheduleAgentClient
    {
      get { return _scheduleAgentClient; }
      set
      {
        _scheduleAgentClient = value;
        OnPropertyChanged();
      }
    }

    public bool SendNoAppointmentNotifications
    {
      get { return _sendNoAppointmentNotifications; }
      set
      {
        _sendNoAppointmentNotifications = value;
        OnPropertyChanged();
      }
    }

    public bool UseEventCategoryColorAndMapFromCalendarColor
    {
      get { return _useEventCategoryColorAndMapFromCalendarColor; }
      set
      {
        _useEventCategoryColorAndMapFromCalendarColor = value;
        OnPropertyChanged();
        // ReSharper disable once ExplicitCallerInfoArgument
        OnPropertyChanged (nameof (UseEventCategoryAsFilterAndMapColor));
      }
    }



    public void SetOptions (CalDavSynchronizer.Contracts.Options options)
    {
      var mappingConfiguration = options.MappingConfiguration as EventMappingConfiguration ?? new EventMappingConfiguration();

      CategoryShortcutKey = mappingConfiguration.CategoryShortcutKey;
      CreateEventsInUtc = mappingConfiguration.CreateEventsInUTC;
      EventCategory = mappingConfiguration.EventCategory;
      EventCategoryColor = mappingConfiguration.EventCategoryColor;
      InvertEventCategoryFilter = mappingConfiguration.InvertEventCategoryFilter;
      MapAttendees = mappingConfiguration.MapAttendees;
      MapBody = mappingConfiguration.MapBody;
      MapClassConfidentialToSensitivityPrivate = mappingConfiguration.MapClassConfidentialToSensitivityPrivate;
      MapReminder = mappingConfiguration.MapReminder;
      MapSensitivityPrivateToClassConfidential = mappingConfiguration.MapSensitivityPrivateToClassConfidential;
      ScheduleAgentClient = mappingConfiguration.ScheduleAgentClient;
      SendNoAppointmentNotifications = mappingConfiguration.SendNoAppointmentNotifications;
      UseEventCategoryColorAndMapFromCalendarColor = mappingConfiguration.UseEventCategoryColorAndMapFromCalendarColor;
    }

    public void FillOptions (CalDavSynchronizer.Contracts.Options options)
    {
      options.MappingConfiguration = new EventMappingConfiguration
                                     {
                                         CategoryShortcutKey = _categoryShortcutKey,
                                         CreateEventsInUTC = _createEventsInUtc,
                                         EventCategory = _eventCategory,
                                         EventCategoryColor = _eventCategoryColor,
                                         InvertEventCategoryFilter = _invertEventCategoryFilter,
                                         MapAttendees = _mapAttendees,
                                         MapBody = _mapBody,
                                         MapClassConfidentialToSensitivityPrivate = _mapClassConfidentialToSensitivityPrivate,
                                         MapReminder = _mapReminder,
                                         MapSensitivityPrivateToClassConfidential = _mapSensitivityPrivateToClassConfidential,
                                         ScheduleAgentClient = _scheduleAgentClient,
                                         SendNoAppointmentNotifications = _sendNoAppointmentNotifications,
                                         UseEventCategoryColorAndMapFromCalendarColor = _useEventCategoryColorAndMapFromCalendarColor
                                     };
    }

    public string Name => "Event mapping configuration";

    public bool Validate (StringBuilder errorBuilder)
    {
      return true;
    }


    public IEnumerable<IOptionsViewModel> SubOptions => new IOptionsViewModel[] { };

    public static EventMappingConfigurationViewModel DesignInstance = new EventMappingConfigurationViewModel
                                                                      {
                                                                          CategoryShortcutKey = OlCategoryShortcutKey.olCategoryShortcutKeyCtrlF4,
                                                                          CreateEventsInUtc = true,
                                                                          EventCategory = "TheCategory",
                                                                          EventCategoryColor = OlCategoryColor.olCategoryColorDarkMaroon,
                                                                          InvertEventCategoryFilter = true,
                                                                          MapAttendees = true,
                                                                          MapBody = true,
                                                                          MapClassConfidentialToSensitivityPrivate = true,
                                                                          MapReminder = ReminderMapping.JustUpcoming,
                                                                          MapSensitivityPrivateToClassConfidential = true,
                                                                          ScheduleAgentClient = true,
                                                                          SendNoAppointmentNotifications = true,
                                                                          UseEventCategoryColorAndMapFromCalendarColor = true
                                                                      };
  }
}