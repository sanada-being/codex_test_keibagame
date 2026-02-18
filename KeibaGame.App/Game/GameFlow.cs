namespace KeibaGame.App;

internal enum FlowOutcome
{
    ReturnToTitle,
    ExitApplication
}

internal enum ResultAction
{
    NextRace,
    EndGame
}

internal static class GameFlow
{
    /// <summary>ゲーム本編の画面遷移と進行を実行します。</summary>
    /// <param name="owner">子画面を表示する親ウィンドウです。</param>
    /// <param name="session">ゲーム状態を保持するセッションです。</param>
    /// <param name="startNew"><c>true</c> の場合は新規開始として初期化します。</param>
    /// <returns>タイトルへ戻るか、アプリ終了かの進行結果です。</returns>
    public static FlowOutcome Run(IWin32Window owner, GameSession session, bool startNew)
    {
        if (startNew)
        {
            session.Reset();
            GameSaveService.Delete();
        }

        var random = new Random();

        while (true)
        {
            var horses = RaceFactory.CreateRace(random);
            Horse? selectedHorse;
            int betAmount;

            while (true)
            {
                using var selectForm = new HorseSelectForm(session, horses);
                if (selectForm.ShowDialog(owner) != DialogResult.OK || selectForm.SelectedHorse is null)
                {
                    return FlowOutcome.ReturnToTitle;
                }

                selectedHorse = selectForm.SelectedHorse;

                using var betForm = new BetForm(session, selectedHorse);
                var betResult = betForm.ShowDialog(owner);
                if (betResult == DialogResult.OK)
                {
                    betAmount = betForm.BetAmount;
                    break;
                }
            }

            using (var ticketForm = new TicketForm(session, selectedHorse!, betAmount))
            {
                if (ticketForm.ShowDialog(owner) != DialogResult.OK)
                {
                    return FlowOutcome.ReturnToTitle;
                }
            }

            session.Balance -= betAmount;

            RaceResult raceResult;
            using (var raceForm = new RaceForm(session, selectedHorse!, betAmount, horses))
            {
                if (raceForm.ShowDialog(owner) != DialogResult.OK || raceForm.Result is null)
                {
                    return FlowOutcome.ReturnToTitle;
                }

                raceResult = raceForm.Result;
            }

            var selected = horses.First(h => h.Number == selectedHorse!.Number);
            var hit = selected.Rank == 1;
            var payout = hit ? (int)Math.Floor(betAmount * selected.Odds) : 0;
            session.Balance += payout;
            session.RecordRace(betAmount, payout, hit);
            GameSaveService.Save(session);

            ResultAction action;
            using (var resultForm = new ResultForm(session, selected, betAmount, payout, raceResult))
            {
                if (resultForm.ShowDialog(owner) != DialogResult.OK)
                {
                    return FlowOutcome.ReturnToTitle;
                }

                action = resultForm.Action;
            }

            if (session.IsClear)
            {
                GameSaveService.Delete();
                using var clearForm = new ClearForm();
                clearForm.ShowDialog(owner);
                return FlowOutcome.ReturnToTitle;
            }

            if (action == ResultAction.EndGame)
            {
                return FlowOutcome.ExitApplication;
            }

            if (session.IsBankrupt)
            {
                GameSaveService.Delete();
                using var bankruptForm = new BankruptForm();
                bankruptForm.ShowDialog(owner);
                return FlowOutcome.ReturnToTitle;
            }
        }
    }
}

