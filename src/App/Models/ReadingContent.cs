namespace WellMind.Models;

public static class ReadingContent
{
    public static IReadOnlyList<ReadingArticle> All { get; } = new List<ReadingArticle>
    {
        new()
        {
            Id = "anxiety",
            Title = "What is anxiety?",
            Body = """
Anxiety is your body trying to protect you.

It’s an alarm system designed to notice danger and prepare you to respond. Sometimes that alarm becomes sensitive and turns on even when there’s no immediate threat.

When anxiety shows up, you might feel tightness in your chest, racing thoughts, or a sense that something is wrong. This doesn’t mean anything bad is happening — it means your nervous system is doing its job a little too well.

Anxiety is common. It comes and goes. And it doesn’t define who you are.

You don’t need to make it disappear right now. Understanding it is often enough.
"""
        },
        new()
        {
            Id = "stress",
            Title = "What causes stress?",
            Body = """
Stress happens when your mind or body feels overloaded.

This can come from work, relationships, finances, health concerns, or simply having too many things to think about at once.

Stress isn’t a failure. It’s a response to pressure. Even good things — like growth or change — can create stress.

Some days your system handles stress easily. Other days, it doesn’t. Both are normal.

Noticing stress without judging yourself is a powerful first step.
"""
        },
        new()
        {
            Id = "spirals",
            Title = "Why thoughts spiral sometimes",
            Body = """
Your brain is very good at pattern-finding.

When it senses uncertainty, it may repeat the same thoughts over and over, trying to find clarity or safety. This can feel exhausting and hard to stop.

Spiraling thoughts aren’t a sign that something is wrong with you. They’re a sign that your mind is searching for reassurance.

You don’t have to solve every thought. Sometimes letting them pass is enough.

Thoughts are experiences — not commands.
"""
        },
        new()
        {
            Id = "sleep",
            Title = "Why sleep affects how you feel",
            Body = """
Sleep helps your brain reset.

When you don’t get enough rest, emotions can feel louder and harder to manage. Things that normally feel manageable may suddenly feel overwhelming.

This isn’t weakness. It’s biology.

Even small improvements in rest can gently shift how you feel over time.

If sleep has been difficult lately, be kind to yourself. Your body is doing its best.
"""
        },
        new()
        {
            Id = "overwhelmed",
            Title = "When everything feels like too much",
            Body = """
Sometimes life feels heavy without a clear reason.

You might feel tired, numb, or emotionally full — like there’s no room for one more thing. This happens to many people, especially during long periods of stress or change.

You don’t need to fix your life today.

Pausing. Reading. Breathing. These small moments still matter.

Feeling overwhelmed doesn’t mean you’re failing — it means you’re human.
"""
        }
    };
}
