using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    public DbSet<Event> Events { set; get; }
    public DbSet<Payment> Payments { set; get; }
    public DbSet<Seat> Seats { set; get; }
    public DbSet<SeatGroup> SeatGroups { set; get; }
    public DbSet<SeatingChart> SeatingCharts { set; get; }
    public DbSet<Ticket> Tickets { set; get; }
    public DbSet<TicketGroup> TicketGroups { set; get; }
    public DbSet<User> Users { set; get; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>()
        .HasOne(tk => tk.User)
        .WithMany(us => us.Tickets)
        .HasForeignKey(tk => tk.UserId)
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Ticket>()
        .HasOne(tk => tk.Seat)
        .WithOne(s => s.Ticket)
        .HasForeignKey<Ticket>(tk => tk.SeatId)
        .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Ticket>()
        .HasOne(tk => tk.Event)
        .WithMany(e => e.Tickets)
        .HasForeignKey(tk => tk.EventId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
        .HasOne(pm => pm.Ticket)
        .WithOne(tk => tk.Payment)
        .HasForeignKey<Payment>(pm=>pm.TicketId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Event>()
        .HasOne(e => e.User)
        .WithMany(us => us.Events)
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Event>()
        .HasOne(e => e.SeatingChart)
        .WithMany(stc => stc.Events)
        .HasForeignKey(e => e.SeatingChartId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SeatGroup>()
        .HasOne(sg => sg.SeatingChart)
        .WithMany(stc => stc.SeatGroups)
        .HasForeignKey(sg => sg.SeatingChartId)
         .OnDelete(DeleteBehavior.NoAction); ;

        modelBuilder.Entity<Seat>()
        .HasOne(s => s.SeatGroup)
        .WithMany(sg => sg.Seats)
        .HasForeignKey(s => s.SeatGroupId)
         .OnDelete(DeleteBehavior.NoAction);


        // modelBuilder.Entity<SeatingChart>()
        // .HasOne(stc => stc.User)
        // .WithMany(us => us.SeatingCharts)
        // .HasForeignKey(stc => stc.UserId)
        // .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<TicketGroup>()
        .HasMany(tkg => tkg.SeatGroups)
        .WithOne(sg => sg.TicketGroup)
        .HasForeignKey(tkg => tkg.TicketGroupId)
        .OnDelete(DeleteBehavior.SetNull);
    }
}