const _ = require('lodash/fp');
const args = require('yargs').argv;
const gulp = require('gulp');
const gutil = require('gulp-util');
const path = require('path');
const proc = require('child_process');
const usage = require('gulp-help-doc');

gulp.task('help', () => usage(gulp));

/**
 * Run Raven.Assure.Test xUnit tests.
 * Same as `dotnet test src/Raven.Assure.Test/`
 * @task {test}
 */
gulp.task('test', function testRavenAssure(done) {
   var testProcess = proc
      .spawn('dotnet', ['test'], {
         cwd: path.join('src', 'Raven.Assure.Test')
      });

   // dotnet CLI adds line break, so when combined with log it adds an additional empty line.
   // This replaces that line break with an empty string.
   var removeLineBreaks = (msg) => msg.replace(/(\r\n|\r|\n)/g, '');
   var toString = (msg) => msg.toString();
   var logDatar = _.flow(toString, removeLineBreaks, gutil.log);
   var logError = _.flow(gutil.colors.red, logDatar);

   testProcess.stdout.on('data', logDatar);
   testProcess.stderr.on('data', logError);
   testProcess.on('close', done);
});
