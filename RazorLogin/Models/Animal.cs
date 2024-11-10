﻿using System;
using System.Collections.Generic;

namespace RazorLogin.Models;

public partial class Animal
{
    public int AnimalId { get; set; }

    public string? AnimalName { get; set; }

    public string? AnimalSex { get; set; }

    public DateOnly? AnimalDob { get; set; }

    public string? Species { get; set; }

    public int? Weight { get; set; }

    public string? Diet { get; set; }

    public DateOnly? ArrivalDate { get; set; }

    public int EnclosureId { get; set; }

    public string? MedicalNotes { get; set; }

    public string? EndangeredStatus { get; set; }

    public TimeOnly? FeedingTime { get; set; }

    public int ZookeeperId { get; set; }

    public virtual Enclosure? Enclosure { get; set; } = null!;  //added ? on 11/10 in order to allow editing of animals

    public virtual Zookeeper? Zookeeper { get; set; } = null!;  //added ? on 11/10 in order to allow editing of animals
}
