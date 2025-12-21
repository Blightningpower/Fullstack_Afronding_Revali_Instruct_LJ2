[HttpGet]
public async Task<ActionResult<IEnumerable<PatientListItemDto>>> GetPatients(CancellationToken ct)
{
    var patients = await _context.Patients
        .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
        .Select(p => new PatientListItemDto
        {
            Id = p.Id,
            FullName = p.FirstName + " " + p.LastName,
            StartDate = p.StartDate == DateTime.MinValue ? null : p.StartDate,
            Status = p.Status,
            StatusLabel = p.Status switch
            {
                PatientStatus.IntakePlanned => "Intake gepland",
                PatientStatus.Active        => "Actief",
                PatientStatus.Completed     => "Afgerond",
                PatientStatus.OnHold        => "On hold",
                _                           => "Onbekend"
            }
        })
        .ToListAsync(ct);

    return Ok(patients);
}