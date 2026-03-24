internal static class AstInspector
{
  public static bool ContainsArgsUsage(ProgramNode program)
  {
    return ContainsArgsUsageInStatements(program.Statements);
  }

  private static bool ContainsArgsUsageInStatements(List<Statement> statements)
  {
    foreach (var statement in statements)
    {
      if (ContainsArgsUsageInStatement(statement))
      {
        return true;
      }
    }

    return false;
  }

  private static bool ContainsArgsUsageInStatement(Statement statement)
  {
    return statement switch
    {
      ArgsDefineStatement => true,
      ArgsShowHelpStatement => true,
      ExpressionStatement es => ContainsArgsUsageInExpression(es.Expression),
      VariableDeclaration vd => ContainsArgsUsageInExpression(vd.Value),
      FunctionDeclaration fd => ContainsArgsUsageInStatements(fd.Body),
      ConsoleLog cl => ContainsArgsUsageInExpression(cl.Message),
      ReturnStatement rs => rs.Value != null && ContainsArgsUsageInExpression(rs.Value),
      IfStatement ifs => ContainsArgsUsageInExpression(ifs.Condition)
        || ContainsArgsUsageInStatements(ifs.ThenBody)
        || ContainsArgsUsageInStatements(ifs.ElseBody),
      ExitStatement es => ContainsArgsUsageInExpression(es.ExitCode),
      ForLoop fl => ContainsArgsUsageInStatement(fl.Initializer)
        || ContainsArgsUsageInExpression(fl.Condition)
        || ContainsArgsUsageInExpression(fl.Update)
        || ContainsArgsUsageInStatements(fl.Body),
      ForInLoop fil => ContainsArgsUsageInExpression(fil.Iterable)
        || ContainsArgsUsageInStatements(fil.Body),
      WhileStatement ws => ContainsArgsUsageInExpression(ws.Condition)
        || ContainsArgsUsageInStatements(ws.Body),
      SwitchStatement ss => ContainsArgsUsageInExpression(ss.Expression)
        || ss.Cases.Any(c => c.Values.Any(ContainsArgsUsageInExpression) || ContainsArgsUsageInStatements(c.Body))
        || (ss.DefaultCase != null && ContainsArgsUsageInStatements(ss.DefaultCase.Body)),
      TryCatchStatement tc => ContainsArgsUsageInStatements(tc.TryBody)
        || ContainsArgsUsageInStatements(tc.CatchBody),
      DeferStatement ds => ContainsArgsUsageInStatement(ds.Statement),
      FsWriteFileStatement ws => ContainsArgsUsageInExpression(ws.FilePath)
        || ContainsArgsUsageInExpression(ws.Content),
      FsCopyStatement cs => ContainsArgsUsageInExpression(cs.SourcePath)
        || ContainsArgsUsageInExpression(cs.TargetPath),
      FsMoveStatement ms => ContainsArgsUsageInExpression(ms.SourcePath)
        || ContainsArgsUsageInExpression(ms.TargetPath),
      FsRenameStatement rs => ContainsArgsUsageInExpression(rs.OldName)
        || ContainsArgsUsageInExpression(rs.NewName),
      FsDeleteStatement ds => ContainsArgsUsageInExpression(ds.Path),
      TemplateUpdateStatement ts => ContainsArgsUsageInExpression(ts.SourceFilePath)
        || ContainsArgsUsageInExpression(ts.TargetFilePath),
      ImportStatement or ConsoleClearStatement or ScriptEnableDebugStatement
        or ScriptDisableDebugStatement or ScriptDisableGlobbingStatement
        or ScriptEnableGlobbingStatement or ScriptExitOnErrorStatement
        or ScriptContinueOnErrorStatement or ScriptDescriptionStatement
        or RawStatement or StructuredTypeDeclaration or BreakStatement
        or ContinueStatement or TimerStartStatement => false,
      _ => false,
    };
  }

  private static bool ContainsArgsUsageInExpression(Expression expression)
  {
    return expression switch
    {
      ArgsHasExpression => true,
      ArgsGetExpression => true,
      ArgsAllExpression => true,
      BinaryExpression be => ContainsArgsUsageInExpression(be.Left)
        || ContainsArgsUsageInExpression(be.Right),
      UnaryExpression ue => ContainsArgsUsageInExpression(ue.Operand),
      TernaryExpression te => ContainsArgsUsageInExpression(te.Condition)
        || ContainsArgsUsageInExpression(te.TrueExpression)
        || ContainsArgsUsageInExpression(te.FalseExpression),
      ParenthesizedExpression pe => ContainsArgsUsageInExpression(pe.Inner),
      StringInterpolationExpression si => si.Parts.Any(p => p is Expression e && ContainsArgsUsageInExpression(e)),
      FunctionCall fc => fc.Arguments.Any(ContainsArgsUsageInExpression),
      ParallelFunctionCall pfc => pfc.Arguments.Any(ContainsArgsUsageInExpression),
      AssignmentExpression ae => ContainsArgsUsageInExpression(ae.Left)
        || ContainsArgsUsageInExpression(ae.Right),
      ArrayLiteral al => al.Elements.Any(ContainsArgsUsageInExpression),
      ArrayAccess aa => ContainsArgsUsageInExpression(aa.Array)
        || ContainsArgsUsageInExpression(aa.Index),
      ArrayLength al => ContainsArgsUsageInExpression(al.Array),
      ArrayIsEmpty ai => ContainsArgsUsageInExpression(ai.Array),
      ArrayContains ac => ContainsArgsUsageInExpression(ac.Array)
        || ContainsArgsUsageInExpression(ac.Item),
      ArrayReverse ar => ContainsArgsUsageInExpression(ar.Array),
      ArrayJoinExpression aj => ContainsArgsUsageInExpression(aj.Array)
        || ContainsArgsUsageInExpression(aj.Separator),
      ArraySortExpression asrt => ContainsArgsUsageInExpression(asrt.Array)
        || (asrt.SortOrder != null && ContainsArgsUsageInExpression(asrt.SortOrder)),
      ArrayMergeExpression am => ContainsArgsUsageInExpression(am.Array1)
        || ContainsArgsUsageInExpression(am.Array2),
      ArrayShuffleExpression ash => ContainsArgsUsageInExpression(ash.Array),
      ArrayUniqueExpression au => ContainsArgsUsageInExpression(au.Array),
      ArrayForEachExpression afe => ContainsArgsUsageInExpression(afe.Array)
        || ContainsArgsUsageInLambda(afe.Callback),
      ArrayMapExpression ame => ContainsArgsUsageInExpression(ame.Array)
        || ContainsArgsUsageInLambda(ame.Callback),
      ArrayFilterExpression afilt => ContainsArgsUsageInExpression(afilt.Array)
        || ContainsArgsUsageInLambda(afilt.Callback),
      ArrayReduceExpression are => ContainsArgsUsageInExpression(are.Array)
        || ContainsArgsUsageInLambda(are.Callback)
        || (are.InitialValue != null && ContainsArgsUsageInExpression(are.InitialValue)),
      ArrayFindExpression afnd => ContainsArgsUsageInExpression(afnd.Array)
        || ContainsArgsUsageInLambda(afnd.Callback),
      ArraySomeExpression asm => ContainsArgsUsageInExpression(asm.Array)
        || ContainsArgsUsageInLambda(asm.Callback),
      ArrayEveryExpression aev => ContainsArgsUsageInExpression(aev.Array)
        || ContainsArgsUsageInLambda(aev.Callback),
      LambdaExpression le => ContainsArgsUsageInLambda(le),
      ObjectLiteralExpression ol => ol.Properties.Any(p => ContainsArgsUsageInExpression(p.Value)),
      ObjectPropertyAccessExpression opa => ContainsArgsUsageInExpression(opa.Object),
      PostIncrementExpression pie => ContainsArgsUsageInExpression(pie.Operand),
      PostDecrementExpression pde => ContainsArgsUsageInExpression(pde.Operand),
      PreIncrementExpression prie => ContainsArgsUsageInExpression(prie.Operand),
      PreDecrementExpression prde => ContainsArgsUsageInExpression(prde.Operand),
      ConsolePromptYesNoExpression cp => ContainsArgsUsageInExpression(cp.PromptText),
      ConsoleShowMessageExpression csm => ContainsArgsUsageInExpression(csm.Title)
        || ContainsArgsUsageInExpression(csm.Message),
      ConsoleShowInfoExpression csi => ContainsArgsUsageInExpression(csi.Title)
        || ContainsArgsUsageInExpression(csi.Message),
      ConsoleShowWarningExpression csw => ContainsArgsUsageInExpression(csw.Title)
        || ContainsArgsUsageInExpression(csw.Message),
      ConsoleShowErrorExpression cse => ContainsArgsUsageInExpression(cse.Title)
        || ContainsArgsUsageInExpression(cse.Message),
      ConsoleShowSuccessExpression css => ContainsArgsUsageInExpression(css.Title)
        || ContainsArgsUsageInExpression(css.Message),
      ConsoleShowChoiceExpression csc => ContainsArgsUsageInExpression(csc.Title)
        || ContainsArgsUsageInExpression(csc.Message)
        || ContainsArgsUsageInExpression(csc.Options)
        || (csc.DefaultIndex != null && ContainsArgsUsageInExpression(csc.DefaultIndex)),
      ConsoleShowMultiChoiceExpression csmc => ContainsArgsUsageInExpression(csmc.Title)
        || ContainsArgsUsageInExpression(csmc.Message)
        || ContainsArgsUsageInExpression(csmc.Options)
        || (csmc.DefaultSelected != null && ContainsArgsUsageInExpression(csmc.DefaultSelected)),
      ConsoleShowConfirmExpression cscf => ContainsArgsUsageInExpression(cscf.Title)
        || ContainsArgsUsageInExpression(cscf.Message)
        || (cscf.DefaultButton != null && ContainsArgsUsageInExpression(cscf.DefaultButton)),
      ConsoleShowProgressExpression csp => ContainsArgsUsageInExpression(csp.Title)
        || ContainsArgsUsageInExpression(csp.Message)
        || ContainsArgsUsageInExpression(csp.Percent)
        || (csp.CanCancel != null && ContainsArgsUsageInExpression(csp.CanCancel)),
      ConsolePromptTextExpression cpt => ContainsArgsUsageInExpression(cpt.Prompt)
        || (cpt.DefaultValue != null && ContainsArgsUsageInExpression(cpt.DefaultValue))
        || (cpt.ValidationPattern != null && ContainsArgsUsageInExpression(cpt.ValidationPattern)),
      ConsolePromptPasswordExpression cpp => ContainsArgsUsageInExpression(cpp.Prompt),
      ConsolePromptNumberExpression cpn => ContainsArgsUsageInExpression(cpn.Prompt)
        || (cpn.MinValue != null && ContainsArgsUsageInExpression(cpn.MinValue))
        || (cpn.MaxValue != null && ContainsArgsUsageInExpression(cpn.MaxValue))
        || (cpn.DefaultValue != null && ContainsArgsUsageInExpression(cpn.DefaultValue)),
      ConsolePromptFileExpression cpf => ContainsArgsUsageInExpression(cpf.Prompt)
        || (cpf.Filter != null && ContainsArgsUsageInExpression(cpf.Filter)),
      ConsolePromptDirectoryExpression cpd => ContainsArgsUsageInExpression(cpd.Prompt)
        || (cpd.DefaultPath != null && ContainsArgsUsageInExpression(cpd.DefaultPath)),
      UtilityRandomExpression ur => (ur.MinValue != null && ContainsArgsUsageInExpression(ur.MinValue))
        || (ur.MaxValue != null && ContainsArgsUsageInExpression(ur.MaxValue)),
      UtilityHashExpression uh => ContainsArgsUsageInExpression(uh.Text)
        || (uh.Algorithm != null && ContainsArgsUsageInExpression(uh.Algorithm)),
      UtilityBase64EncodeExpression ub => ContainsArgsUsageInExpression(ub.Text),
      UtilityBase64DecodeExpression ubd => ContainsArgsUsageInExpression(ubd.Text),
      OsIsInstalledExpression oi => ContainsArgsUsageInExpression(oi.AppName),
      WebGetExpression wg => ContainsArgsUsageInExpression(wg.Url),
      WebDeleteExpression wd => ContainsArgsUsageInExpression(wd.Url)
        || (wd.Options != null && ContainsArgsUsageInExpression(wd.Options)),
      WebPostExpression wp => ContainsArgsUsageInExpression(wp.Url)
        || ContainsArgsUsageInExpression(wp.Data)
        || (wp.Options != null && ContainsArgsUsageInExpression(wp.Options)),
      WebPutExpression wpu => ContainsArgsUsageInExpression(wpu.Url)
        || ContainsArgsUsageInExpression(wpu.Data)
        || (wpu.Options != null && ContainsArgsUsageInExpression(wpu.Options)),
      WebSpeedtestExpression ws => ContainsArgsUsageInExpression(ws.Url)
        || (ws.Options != null && ContainsArgsUsageInExpression(ws.Options)),
      WebDownloadExpression wdl => ContainsArgsUsageInExpression(wdl.Url)
        || ContainsArgsUsageInExpression(wdl.OutputPath),
      FsDirnameExpression fd => ContainsArgsUsageInExpression(fd.Path),
      FsFileNameExpression ff => ContainsArgsUsageInExpression(ff.Path),
      FsExtensionExpression fe => ContainsArgsUsageInExpression(fe.Path),
      FsParentDirNameExpression fp => ContainsArgsUsageInExpression(fp.Path),
      FsExistsExpression fex => ContainsArgsUsageInExpression(fex.Path),
      FsReadFileExpression fr => ContainsArgsUsageInExpression(fr.FilePath),
      FsWriteFileExpressionPlaceholder fwp => ContainsArgsUsageInExpression(fwp.FilePath)
        || ContainsArgsUsageInExpression(fwp.Content),
      FsCopyExpression fce => ContainsArgsUsageInExpression(fce.SourcePath)
        || ContainsArgsUsageInExpression(fce.TargetPath),
      FsMoveExpression fme => ContainsArgsUsageInExpression(fme.SourcePath)
        || ContainsArgsUsageInExpression(fme.TargetPath),
      FsRenameExpression fre => ContainsArgsUsageInExpression(fre.OldName)
        || ContainsArgsUsageInExpression(fre.NewName),
      FsDeleteExpression fde => ContainsArgsUsageInExpression(fde.Path),
      FsChmodExpression fcm => ContainsArgsUsageInExpression(fcm.Path)
        || ContainsArgsUsageInExpression(fcm.Permissions),
      FsChownExpression fco => ContainsArgsUsageInExpression(fco.Path)
        || ContainsArgsUsageInExpression(fco.Owner)
        || (fco.Group != null && ContainsArgsUsageInExpression(fco.Group)),
      FsFindExpression ffn => ContainsArgsUsageInExpression(ffn.SearchPath)
        || (ffn.NamePattern != null && ContainsArgsUsageInExpression(ffn.NamePattern)),
      FsWatchExpression fw => ContainsArgsUsageInExpression(fw.Path)
        || ContainsArgsUsageInExpression(fw.Callback)
        || (fw.Options != null && ContainsArgsUsageInExpression(fw.Options)),
      FsCreateTempFolderExpression fctf => (fctf.Prefix != null && ContainsArgsUsageInExpression(fctf.Prefix))
        || (fctf.BaseDir != null && ContainsArgsUsageInExpression(fctf.BaseDir)),
      StringNamespaceCallExpression snc => snc.Arguments.Any(ContainsArgsUsageInExpression),
      ArrayNamespaceCallExpression anc => anc.Arguments.Any(ContainsArgsUsageInExpression),
      ProcessStartExpression ps => ContainsArgsUsageInExpression(ps.Command)
        || (ps.Cwd != null && ContainsArgsUsageInExpression(ps.Cwd))
        || (ps.Input != null && ContainsArgsUsageInExpression(ps.Input))
        || (ps.Output != null && ContainsArgsUsageInExpression(ps.Output))
        || (ps.Error != null && ContainsArgsUsageInExpression(ps.Error)),
      ProcessIsRunningExpression pr => ContainsArgsUsageInExpression(pr.Pid),
      ProcessWaitForExitExpression pw => ContainsArgsUsageInExpression(pw.Pid)
        || (pw.Timeout != null && ContainsArgsUsageInExpression(pw.Timeout)),
      ProcessKillExpression pk => ContainsArgsUsageInExpression(pk.Pid)
        || (pk.Signal != null && ContainsArgsUsageInExpression(pk.Signal)),
      GitResetToCommitExpression gr => ContainsArgsUsageInExpression(gr.CommitHash),
      DockerRunExpression dr => ContainsArgsUsageInExpression(dr.Image)
        || (dr.Name != null && ContainsArgsUsageInExpression(dr.Name))
        || (dr.Ports != null && ContainsArgsUsageInExpression(dr.Ports))
        || (dr.Volumes != null && ContainsArgsUsageInExpression(dr.Volumes)),
      DockerStopExpression ds => ContainsArgsUsageInExpression(ds.Container),
      DockerRemoveExpression dre => ContainsArgsUsageInExpression(dre.Container),
      DockerRestartExpression drst => ContainsArgsUsageInExpression(drst.Container),
      DockerLogsExpression dl => ContainsArgsUsageInExpression(dl.Container),
      DockerExecExpression de => ContainsArgsUsageInExpression(de.Container)
        || ContainsArgsUsageInExpression(de.Command),
      DockerIsRunningExpression dir => ContainsArgsUsageInExpression(dir.Container),
      DockerBuildExpression db => ContainsArgsUsageInExpression(db.Tag)
        || (db.Path != null && ContainsArgsUsageInExpression(db.Path)),
      DockerPullExpression dp => ContainsArgsUsageInExpression(dp.Image),
      DockerPushExpression dpu => ContainsArgsUsageInExpression(dpu.Image),
      DockerRemoveImageExpression dri => ContainsArgsUsageInExpression(dri.Image),
      DockerImageExistsExpression die => ContainsArgsUsageInExpression(die.Image),
      K8sSetContextExpression ksc => ContainsArgsUsageInExpression(ksc.Name),
      K8sSetNamespaceExpression ksn => ContainsArgsUsageInExpression(ksn.Name),
      K8sGetExpression kg => ContainsArgsUsageInExpression(kg.Resource)
        || (kg.Name != null && ContainsArgsUsageInExpression(kg.Name))
        || (kg.Namespace != null && ContainsArgsUsageInExpression(kg.Namespace)),
      K8sDescribeExpression kd => ContainsArgsUsageInExpression(kd.Resource)
        || ContainsArgsUsageInExpression(kd.Name)
        || (kd.Namespace != null && ContainsArgsUsageInExpression(kd.Namespace)),
      K8sExistsExpression ke => ContainsArgsUsageInExpression(ke.Resource)
        || ContainsArgsUsageInExpression(ke.Name)
        || (ke.Namespace != null && ContainsArgsUsageInExpression(ke.Namespace)),
      K8sLogsExpression kl => ContainsArgsUsageInExpression(kl.Pod)
        || (kl.Container != null && ContainsArgsUsageInExpression(kl.Container))
        || (kl.Tail != null && ContainsArgsUsageInExpression(kl.Tail))
        || (kl.Previous != null && ContainsArgsUsageInExpression(kl.Previous)),
      K8sExecExpression kex => ContainsArgsUsageInExpression(kex.Pod)
        || ContainsArgsUsageInExpression(kex.Command)
        || (kex.Container != null && ContainsArgsUsageInExpression(kex.Container)),
      K8sPortForwardExpression kpf => ContainsArgsUsageInExpression(kpf.Pod)
        || ContainsArgsUsageInExpression(kpf.LocalPort)
        || ContainsArgsUsageInExpression(kpf.RemotePort),
      K8sIsReadyExpression kir => ContainsArgsUsageInExpression(kir.Pod)
        || (kir.Namespace != null && ContainsArgsUsageInExpression(kir.Namespace)),
      K8sScaleExpression ks => ContainsArgsUsageInExpression(ks.Resource)
        || ContainsArgsUsageInExpression(ks.Name)
        || ContainsArgsUsageInExpression(ks.Replicas),
      K8sGetSecretExpression kgs => ContainsArgsUsageInExpression(kgs.Name)
        || (kgs.Key != null && ContainsArgsUsageInExpression(kgs.Key))
        || (kgs.Namespace != null && ContainsArgsUsageInExpression(kgs.Namespace)),
      K8sSetSecretExpression kss => ContainsArgsUsageInExpression(kss.Name)
        || ContainsArgsUsageInExpression(kss.Key)
        || ContainsArgsUsageInExpression(kss.Value),
      K8sTopPodsExpression ktp => ktp.Namespace != null && ContainsArgsUsageInExpression(ktp.Namespace),
      SshConnectExpression sc => ContainsArgsUsageInExpression(sc.Host)
        || (sc.Port != null && ContainsArgsUsageInExpression(sc.Port))
        || (sc.Username != null && ContainsArgsUsageInExpression(sc.Username))
        || (sc.Password != null && ContainsArgsUsageInExpression(sc.Password))
        || (sc.KeyPath != null && ContainsArgsUsageInExpression(sc.KeyPath))
        || (sc.ConfigName != null && ContainsArgsUsageInExpression(sc.ConfigName))
        || (sc.Async != null && ContainsArgsUsageInExpression(sc.Async)),
      SshExecuteExpression se => ContainsArgsUsageInExpression(se.Connection)
        || ContainsArgsUsageInExpression(se.Command),
      SshUploadExpression su => ContainsArgsUsageInExpression(su.Connection)
        || ContainsArgsUsageInExpression(su.LocalPath)
        || ContainsArgsUsageInExpression(su.RemotePath),
      SshDownloadExpression sd => ContainsArgsUsageInExpression(sd.Connection)
        || ContainsArgsUsageInExpression(sd.RemotePath)
        || ContainsArgsUsageInExpression(sd.LocalPath),
      JsonParseExpression jp => ContainsArgsUsageInExpression(jp.JsonString),
      JsonStringifyExpression js => ContainsArgsUsageInExpression(js.JsonObject),
      JsonIsValidExpression jiv => ContainsArgsUsageInExpression(jiv.JsonString),
      JsonGetExpression jg => ContainsArgsUsageInExpression(jg.JsonObject)
        || ContainsArgsUsageInExpression(jg.Path),
      JsonSetExpression jse => ContainsArgsUsageInExpression(jse.JsonObject)
        || ContainsArgsUsageInExpression(jse.Path)
        || ContainsArgsUsageInExpression(jse.Value),
      JsonHasExpression jh => ContainsArgsUsageInExpression(jh.JsonObject)
        || ContainsArgsUsageInExpression(jh.Path),
      JsonDeleteExpression jde => ContainsArgsUsageInExpression(jde.JsonObject)
        || ContainsArgsUsageInExpression(jde.Path),
      JsonKeysExpression jk => ContainsArgsUsageInExpression(jk.JsonObject),
      JsonValuesExpression jv => ContainsArgsUsageInExpression(jv.JsonObject),
      JsonMergeExpression jm => ContainsArgsUsageInExpression(jm.JsonObject1)
        || ContainsArgsUsageInExpression(jm.JsonObject2),
      YamlParseExpression yp => ContainsArgsUsageInExpression(yp.YamlString),
      YamlStringifyExpression ys => ContainsArgsUsageInExpression(ys.YamlObject),
      YamlIsValidExpression yiv => ContainsArgsUsageInExpression(yiv.YamlString),
      YamlGetExpression yg => ContainsArgsUsageInExpression(yg.YamlObject)
        || ContainsArgsUsageInExpression(yg.Path),
      YamlSetExpression yse => ContainsArgsUsageInExpression(yse.YamlObject)
        || ContainsArgsUsageInExpression(yse.Path)
        || ContainsArgsUsageInExpression(yse.Value),
      YamlHasExpression yh => ContainsArgsUsageInExpression(yh.YamlObject)
        || ContainsArgsUsageInExpression(yh.Path),
      YamlDeleteExpression yde => ContainsArgsUsageInExpression(yde.YamlObject)
        || ContainsArgsUsageInExpression(yde.Path),
      YamlKeysExpression yk => ContainsArgsUsageInExpression(yk.YamlObject),
      YamlValuesExpression yv => ContainsArgsUsageInExpression(yv.YamlObject),
      YamlMergeExpression ym => ContainsArgsUsageInExpression(ym.YamlObject1)
        || ContainsArgsUsageInExpression(ym.YamlObject2),
      ValidateIsEmailExpression ve => ContainsArgsUsageInExpression(ve.Email),
      ValidateIsURLExpression vu => ContainsArgsUsageInExpression(vu.Url),
      ValidateIsUUIDExpression vuid => ContainsArgsUsageInExpression(vuid.Uuid),
      ValidateIsEmptyExpression vem => ContainsArgsUsageInExpression(vem.Value),
      ValidateIsGreaterThanExpression vgt => ContainsArgsUsageInExpression(vgt.Value)
        || ContainsArgsUsageInExpression(vgt.Threshold),
      ValidateIsLessThanExpression vlt => ContainsArgsUsageInExpression(vlt.Value)
        || ContainsArgsUsageInExpression(vlt.Threshold),
      ValidateIsInRangeExpression vir => ContainsArgsUsageInExpression(vir.Value)
        || ContainsArgsUsageInExpression(vir.Min)
        || ContainsArgsUsageInExpression(vir.Max),
      ValidateIsNumericExpression vn => ContainsArgsUsageInExpression(vn.Value),
      ValidateIsAlphaNumericExpression van => ContainsArgsUsageInExpression(van.Value),
      SchedulerCronExpression sce => ContainsArgsUsageInExpression(sce.CronPattern)
        || ContainsArgsUsageInLambda(sce.Job),
      DateFormatExpression df => (df.Timestamp != null && ContainsArgsUsageInExpression(df.Timestamp))
        || (df.Format != null && ContainsArgsUsageInExpression(df.Format)),
      DateParseExpression dpe => ContainsArgsUsageInExpression(dpe.DateString)
        || (dpe.Format != null && ContainsArgsUsageInExpression(dpe.Format)),
      DateDiffExpression dd => ContainsArgsUsageInExpression(dd.Timestamp1)
        || ContainsArgsUsageInExpression(dd.Timestamp2)
        || (dd.Unit != null && ContainsArgsUsageInExpression(dd.Unit)),
      DateAddExpression da => ContainsArgsUsageInExpression(da.Timestamp)
        || ContainsArgsUsageInExpression(da.Amount)
        || ContainsArgsUsageInExpression(da.Unit),
      DateSubtractExpression dsub => ContainsArgsUsageInExpression(dsub.Timestamp)
        || ContainsArgsUsageInExpression(dsub.Amount)
        || ContainsArgsUsageInExpression(dsub.Unit),
      DateDayOfWeekExpression ddw => ddw.Timestamp != null && ContainsArgsUsageInExpression(ddw.Timestamp),
      TemplateUpdateExpression tu => ContainsArgsUsageInExpression(tu.SourceFilePath)
        || ContainsArgsUsageInExpression(tu.TargetFilePath),
      // Leaf expressions with no child expressions
      LiteralExpression or VariableExpression or MultilineStringExpression
        or ConsoleIsSudoExpression or ConsoleIsInteractiveExpression
        or ConsoleGetShellExpression or UtilityUuidExpression
        or OsGetLinuxVersionExpression or OsGetOSExpression
        or ProcessElapsedTimeExpression or ProcessIdExpression
        or ProcessCpuExpression or ProcessMemoryExpression
        or ProcessCommandExpression or ProcessStatusExpression
        or TimerCurrentExpression or TimerStartExpression or TimerStopExpression
        or GitUndoLastCommitExpression or GitStatusExpression
        or GitCurrentBranchExpression or GitIsCleanExpression
        or DockerListExpression or K8sGetContextExpression
        or K8sGetNamespaceExpression or K8sClientVersionExpression
        or K8sServerVersionExpression or K8sTopNodesExpression
        or JsonInstallDependenciesExpression or YamlInstallDependenciesExpression
        or SystemCpuCountExpression or SystemMemoryTotalExpression
        or SystemMemoryUsageExpression or DateNowExpression
        or DateNowMillisExpression => false,
      _ => false,
    };
  }

  private static bool ContainsArgsUsageInLambda(LambdaExpression lambda)
  {
    return ContainsArgsUsageInStatements(lambda.Body);
  }
}
